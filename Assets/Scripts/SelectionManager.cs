using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RTSUI;

namespace RTSManagers
{

    public enum MouseEvent
    {
        Nothing = 0,
        Selection = 1,
        PrefabBuild = 2,
        UnitMove = 3,
        UnitAttack = 4,
        Rally = 5,
    }

    public class SelectionManager : MonoBehaviour
    {
        #region SingletonCode
        private static SelectionManager _instance;
        public static SelectionManager Instance { get { return _instance; } }
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        //single pattern ends here
        #endregion

        public Vector2 boxStart;
        public Vector2 boxEnd;

        public GameObject[] players;
        public bool boxActive = false;
        public Texture selectionBox;

        //mouse selection
        public MouseEvent currentEvent = MouseEvent.Nothing;

        //list of all objects
        public List<SelectableObject> AllObjects;
        //list of all selected objects
        public List<SelectableObject> SelectedObjects;
        //primary selected object
        public SelectableObject PrimarySelectable;
        //currently hit object
        public SelectableObject HitObject;


        //list of all deactivated object
        public List<Queue<SelectableObject>> deactivatedObjects;

        //selected types
        public bool selectedTypeFlag = false;

        //dirty flag for selection changes
        public bool selectionChanged = false;

        //raycasting
        public Vector3 mousePosition;
        private Ray ray;
        private RaycastHit hit;
        public LayerMask selectables;

        //layermask for placements
        public LayerMask groundMask;

        //double click
        private float doubleClickTimeLimit = 0.2f;


        void Start()
        {
            SelectedObjects = new List<SelectableObject>();
            foreach (GameObject player in players)
            {
                AllObjects.Add(player.GetComponent<SelectableObject>());
            }

            //selectables = LayerMask.GetMask("Default");
            selectables = LayerMask.GetMask("Player");
            selectables += LayerMask.GetMask("Background");
            selectables += LayerMask.GetMask("Wall");
            selectables += LayerMask.GetMask("Turret");
            selectables += LayerMask.GetMask("Droid");
            selectables += LayerMask.GetMask("Barracks");

            groundMask = LayerMask.GetMask("Background");

            StartCoroutine(DoubleClickListener());

            //deactivated object pool
            deactivatedObjects = new List<Queue<SelectableObject>>();
            deactivatedObjects.Add(new Queue<SelectableObject>());  // Turret
            deactivatedObjects.Add(new Queue<SelectableObject>());  // Barracks
            deactivatedObjects.Add(new Queue<SelectableObject>());  // Wall

        }

        // Update is called once per frame
        void Update()
        {

            //handle selection box first
            HandleSelectionBox();

            //raycast the mouse
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (currentEvent == MouseEvent.PrefabBuild)
            {
                if (Physics.Raycast(ray, out hit, 500, groundMask))
                {
                    mousePosition = hit.point;
                }

            }
            else
            {
                if (Physics.Raycast(ray, out hit, 500, selectables))
                {
                    mousePosition = hit.point;
                }
            }

            //check to see if anything gets hit
            if (hit.collider && hit.collider.gameObject.tag == "SelectableObject")
            {
                HitObject = hit.collider.gameObject.GetComponent<SelectableObject>();
            }
            else {
                HitObject = null;
            }

            //handle left mouse click events
            HandleLeftMouseClicks();

            //handle right mouse click events
            HandleRightMouseClicks();

            //handle key press
            HandleKeys();

            //handle selection changes
            HandleSelectionChanges();
        }

        public void OnPrefabCreation()
        {
            ClearSelection();
            currentEvent = MouseEvent.PrefabBuild;
        }

        public void ClearSelection()
        {
            if (!SelectedObjects.Count.Equals(0))
            {
                foreach (SelectableObject obj in SelectedObjects)
                {
                    obj.OnDeselect();
                }
                SelectedObjects.Clear();
            }

            selectionChanged = true;
            currentEvent = MouseEvent.Nothing;
            boxStart = Vector2.zero;
            boxEnd = Vector2.zero;
            boxActive = false;

            SwitchPrimarySelected();
        }

        //here is the factory
        public GameObject UseFactoryPattern(Vector3 pos, EntityType type)
        {
            GameObject returnObject;

            switch (type)
            {
                case EntityType.Turret:
                    pos = new Vector3(pos.x, pos.y , pos.z);
                    if (deactivatedObjects[0].Count > 0) {
                        returnObject = deactivatedObjects[0].Dequeue().gameObject;

                        returnObject.SetActive(true);
                        returnObject.GetComponent<SelectableObject>().ResetValues();
                        returnObject.transform.position = pos;
                    }
                    else { 
                        returnObject = GameObject.Instantiate(RTSManager.Instance.turretPrefab, pos, Quaternion.identity);
                        SelectionManager.Instance.AllObjects.Add(returnObject.GetComponent<SelectableObject>());
                    }
                    //Debug.Log(returnObject.GetComponent<SelectableObject>().id);

                    return returnObject;
                    break;
                case EntityType.Barracks:
                    pos = new Vector3(pos.x, pos.y + 0.2f, pos.z);
                    if (deactivatedObjects[1].Count > 0)
                    {
                        returnObject = deactivatedObjects[1].Dequeue().gameObject;

                        returnObject.SetActive(true);
                        returnObject.GetComponent<SelectableObject>().ResetValues();
                        returnObject.transform.position = pos;
                    }
                    else
                    {
                        returnObject = GameObject.Instantiate(RTSManager.Instance.barracksPrefab, pos, Quaternion.identity);
                        SelectionManager.Instance.AllObjects.Add(returnObject.GetComponent<SelectableObject>());
                    }
                    return returnObject;
                    break;
                case EntityType.Wall:
                    pos = new Vector3(pos.x, pos.y + 3.0f, pos.z);
                    if (deactivatedObjects[2].Count > 0)
                    {
                        returnObject = deactivatedObjects[2].Dequeue().gameObject;

                        returnObject.SetActive(true);
                        returnObject.GetComponent<SelectableObject>().ResetValues();
                        returnObject.transform.position = pos;
                    }
                    else
                    {
                        returnObject = GameObject.Instantiate(RTSManager.Instance.wallPrefab, pos, Quaternion.identity);
                        SelectionManager.Instance.AllObjects.Add(returnObject.GetComponent<SelectableObject>());
                    }
                    return returnObject;
                    break;
                default:
                    return new GameObject();
                    break;
            }

        }

        private void HandleSelectionBox()
        {
            //handle box init behaviour
            if (Input.GetMouseButtonDown(0) && boxActive == false && currentEvent != MouseEvent.PrefabBuild)
            {
                boxStart = Input.mousePosition;
                boxActive = true;
            }
            //handle box drag updates
            else if (Input.GetMouseButton(0) && boxActive)
            {
                if (Mathf.Abs(boxStart.x - Input.mousePosition.x) > 15 || Mathf.Abs(boxStart.y - Input.mousePosition.y) > 15)
                {
                    boxEnd = Input.mousePosition;
                }
                else
                {
                    boxEnd = Vector2.zero;
                }
            }
            //handle box release
            if (Input.GetMouseButtonUp(0) && boxActive)
            {
                Vector3 worldSelection1;

                Ray rayCast = Camera.main.ScreenPointToRay(boxStart);
                RaycastHit castHit;
                if (Physics.Raycast(rayCast, out castHit, 500))
                {
                    worldSelection1 = castHit.point;

                    foreach (SelectableObject obj in AllObjects)
                    {
                        if (obj.GetComponent<Transform>().position.x >= Mathf.Min(worldSelection1.x, mousePosition.x) &&
                            obj.GetComponent<Transform>().position.x <= Mathf.Max(worldSelection1.x, mousePosition.x) &&
                            obj.GetComponent<Transform>().position.z >= Mathf.Min(worldSelection1.z, mousePosition.z) &&
                            obj.GetComponent<Transform>().position.z <= Mathf.Max(worldSelection1.z, mousePosition.z) && !SelectedObjects.Contains(obj) &&
                            obj.gameObject.activeSelf)
                        {
                            //Debug.Log(obj.name);
                            SelectedObjects.Add(obj);
                            currentEvent = MouseEvent.Selection;
                            obj.GetComponent<SelectableObject>().OnSelect();
                            selectionChanged = true;
                        }
                    }
                    SwitchPrimarySelected();
                }

                boxStart = Vector2.zero;
                boxEnd = Vector2.zero;
                boxActive = false;
            }
        }

        private void HandleKeys()
        {

            //deactivate preset on shift hold up
            if (Input.GetKeyUp(KeyCode.LeftShift) && currentEvent == MouseEvent.PrefabBuild)
            {
                RTSManager.Instance.prefabObject.SetActive(false);
                ClearSelection();
            }


            //handles primary selectable cycling
            if (Input.GetKeyDown(KeyCode.Tab) && PrimarySelectable != null && currentEvent == MouseEvent.Selection)
            {
                //allows the program to do a full search starting from the flag
                do
                {
                    foreach (SelectableObject obj in SelectedObjects)
                    {
                        //this checks for the first item in the next selectable type, then switches the primary to it
                        if (PrimarySelectable == obj)
                        {
                            //checks if this went full circle
                            if (selectedTypeFlag)
                            {
                                selectedTypeFlag = false;
                                break;
                            }
                            //else, set flag to true
                            selectedTypeFlag = true;
                        }
                        //if a different type is found, and flag is active
                        if (obj.type != PrimarySelectable.type && selectedTypeFlag)
                        {
                            selectedTypeFlag = false;
                            PrimarySelectable = obj;
                        }
                    }
                } while (selectedTypeFlag);                 //my first practical use of do-while =D
            }

        }

        private void HandleLeftMouseClicks()
        {

            //check if anything events need to be handled
            if (Input.GetMouseButtonDown(0))
            {

                if (currentEvent == MouseEvent.PrefabBuild && RTSManager.Instance.prefabObject != null && RTSManager.Instance.prefabObject.GetComponent<ShellPlacement>().placeable)
                {
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        RTSManager.Instance.prefabObject.SetActive(false);
                        ClearSelection();
                    }
                    if (ResourceManager.Instance.Purchase(RTSManager.Instance.prefabType))
                    {
                        RTSManager.Instance.OnPlace(UseFactoryPattern(mousePosition, RTSManager.Instance.prefabType));
                    }
                    else
                    {
                        Debug.Log("NOT ENOUGH CREDITS");

                    }

                } else if (currentEvent == MouseEvent.PrefabBuild && RTSManager.Instance.prefabObject != null && !RTSManager.Instance.prefabObject.GetComponent<ShellPlacement>().placeable) {
                    Debug.Log("INVALID PLACEMENT");
                }
                else if (currentEvent == MouseEvent.UnitMove)
                {
                    //send the mouse location of all objects with the same type as the primary type
                    foreach (SelectableObject obj in SelectedObjects)
                    {
                        if (obj.type == PrimarySelectable.type)
                        {
                            obj.IssueLocation(mousePosition);
                        }
                    }

                    AnimationManager.Instance.PlayMove(mousePosition);
                    RTSManager.Instance.prefabObject.SetActive(false);
                    currentEvent = MouseEvent.Selection;

                }
                else if (currentEvent == MouseEvent.UnitAttack)
                {
                    if (HitObject != null && HitObject.type == EntityType.Player)
                    {
                        switch (PrimarySelectable.type)
                        {
                            //droids will attack tether to enemy
                            case EntityType.Droid:
                                foreach (SelectableObject obj in SelectedObjects)
                                {
                                    if (obj.type == EntityType.Droid)
                                    {
                                        Droid temp = (Droid)obj;
                                        temp.IssueAttack((Player)HitObject);
                                    }
                                }
                                break;
                            //turrets will aim attack enemy
                            case EntityType.Turret:
                                foreach (SelectableObject obj in SelectedObjects)
                                {
                                    if (obj.type == EntityType.Turret)
                                    {
                                        Turret temp = (Turret)obj;
                                        temp.IssueAttack((Player)HitObject);
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                        AnimationManager.Instance.PlayAttack(mousePosition);
                        RTSManager.Instance.prefabObject.SetActive(false);
                        currentEvent = MouseEvent.Selection;
                    }
                    else
                    {
                        switch (PrimarySelectable.type)
                        {
                            //droids will attack tether to enemy
                            case EntityType.Droid:
                                foreach (SelectableObject obj in SelectedObjects)
                                {
                                    if (obj.type == EntityType.Droid)
                                    {
                                        Droid temp = (Droid)obj;
                                        temp.IssueAttack(mousePosition);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        AnimationManager.Instance.PlayAttack(mousePosition);
                        RTSManager.Instance.prefabObject.SetActive(false);
                        currentEvent = MouseEvent.Selection;
                    }

                }
                else if (currentEvent == MouseEvent.Rally)
                {
                    //send the mouse location of all objects with the same type as the primary type
                    foreach (SelectableObject obj in SelectedObjects)
                    {
                        if (obj.type == PrimarySelectable.type)
                        {
                            obj.IssueLocation(mousePosition);
                        }
                    }

                    RTSManager.Instance.prefabObject.SetActive(false);
                    currentEvent = MouseEvent.Selection;

                }
                else
                {

                    //selection checking
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {

                        if (HitObject == null)
                        {
                            foreach (SelectableObject obj in SelectedObjects)
                            {
                                obj.OnDeselect();
                            }
                            SelectedObjects.Clear();
                        }
                        else if (HitObject.gameObject.activeSelf)
                        {
                            //check to see if already selected
                            if (SelectedObjects.Contains(HitObject))
                            {
                                if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
                                {
                                    ClearSelection();
                                    SelectedObjects.Add(HitObject);
                                    SwitchPrimarySelected(HitObject);

                                    currentEvent = MouseEvent.Selection;
                                    HitObject.OnSelect();
                                }
                                //refocus to one selection
                                else
                                {
                                    DeselectItem(HitObject);
                                }
                            }
                            else
                            {
                                if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
                                {
                                    ClearSelection();
                                }

                                SelectedObjects.Add(HitObject);
                                SwitchPrimarySelected(HitObject);

                                currentEvent = MouseEvent.Selection;
                                HitObject.OnSelect();
                            }
                        }
                        //deselect on ground selection, with selection exceptions
                        else if (HitObject.gameObject.tag == "Ground" && !((currentEvent == MouseEvent.PrefabBuild || (currentEvent == MouseEvent.Selection && boxActive)) && Input.GetKey(KeyCode.LeftShift)))
                        {
                            foreach (SelectableObject obj in SelectedObjects)
                            {
                                obj.OnDeselect();
                            }
                            currentEvent = MouseEvent.Nothing;
                            SelectedObjects.Clear();
                        }
                        selectionChanged = true;
                    }
                }
            }
        }

        private void HandleRightMouseClicks()
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                if (currentEvent == MouseEvent.PrefabBuild || currentEvent == MouseEvent.UnitMove || currentEvent == MouseEvent.UnitAttack || currentEvent == MouseEvent.Rally)
                {
                    RTSManager.Instance.prefabObject.SetActive(false);
                    currentEvent = MouseEvent.Selection;
                }

                if (currentEvent == MouseEvent.Selection)
                {

                    //check if enemy selected
                    if (HitObject != null && HitObject.type == EntityType.Player)
                    {
                        switch (PrimarySelectable.type)
                        {
                            //droids will attack tether to enemy
                            case EntityType.Droid:
                                foreach (SelectableObject obj in SelectedObjects)
                                {
                                    if (obj.type == EntityType.Droid)
                                    {
                                        Droid temp = (Droid)obj;
                                        temp.IssueAttack((Player)HitObject);
                                    }
                                }
                                break;
                            //turrets will aim attack enemy
                            case EntityType.Turret:
                                foreach (SelectableObject obj in SelectedObjects)
                                {
                                    if (obj.type == EntityType.Turret)
                                    {
                                        Turret temp = (Turret)obj;
                                        temp.IssueAttack((Player)HitObject);
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                        AnimationManager.Instance.PlayAttack(mousePosition);
                    }
                    else
                    {

                        switch (PrimarySelectable.type)
                        {
                            case EntityType.Droid:
                                AnimationManager.Instance.PlayMove(mousePosition);
                                break;
                            default:
                                break;
                        }

                        //send the mouse location of all objects with the same type as the primary type
                        foreach (SelectableObject obj in SelectedObjects)
                        {
                            if (obj.type == PrimarySelectable.type)
                            {
                                obj.IssueLocation(mousePosition);
                            }
                        }
                    }
                }

            }
        }

        public void OnFocusSelected(SelectableObject obj)
        {
            ClearSelection();
            SelectedObjects.Add(obj);
            SwitchPrimarySelected(obj);

            currentEvent = MouseEvent.Selection;
            obj.OnSelect();

        }



        public void SwitchPrimarySelected(SelectableObject primary = null)
        {
            if (primary == null && SelectedObjects.Count > 0)
            {
                PrimarySelectable = SelectedObjects[SelectedObjects.Count - 1];
            }
            else
            {
                PrimarySelectable = primary;
            }
            //selectionChanged = true;
        }

        //deselects an object
        public void DeselectItem(SelectableObject obj)
        {
            obj.OnDeselect();

            //remove obj
            if (SelectedObjects.Contains(obj))
            {
                SelectedObjects.Remove(obj);
            }

            //exception checking
            if (SelectedObjects.Count > 0)
            {
                if (PrimarySelectable == obj)
                {
                    PrimarySelectable = SelectedObjects[SelectedObjects.Count - 1];
                }
                if (SelectedObjects.Count > 1)
                {
                    SelectionUI.Instance.ProcessUI(false);
                }
            }
            else if(currentEvent != MouseEvent.PrefabBuild)
            {
                ClearSelection();
            }

        }

        private void HandleSelectionChanges()
        {

            //if flag is activated do operations
            if (selectionChanged)
            {
                //Debug.Log("Selection Changed");
                if (SelectedObjects.Count > 1)
                {
                    //here is where the selection UI happens
                    SelectionUI.Instance.ProcessUI(true);
                }

                selectionChanged = false;
            }
        }

        private void OnGUI()
        {
            //used to draw selection box

            if (boxStart != Vector2.zero && boxEnd != Vector2.zero)
            {
                GUI.DrawTexture(new Rect(boxStart.x, Screen.height - boxStart.y, boxEnd.x - boxStart.x, -1 * ((Screen.height - boxStart.y) - (Screen.height - boxEnd.y))), selectionBox);
            }
        }

        private IEnumerator DoubleClickListener()
        {
            while (enabled)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    yield return ClickEvent();
                }
                yield return null;
            }
        }

        private IEnumerator ClickEvent()
        {
            //pause a frame so you don't pick up the same mouse down event.
            yield return new WaitForEndOfFrame();

            float count = 0f;
            while (count < doubleClickTimeLimit)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    DoubleClick();
                    yield break;
                }
                count += Time.deltaTime;
                yield return null;
            }
        }

        private void DoubleClick()
        {
            //Debug.Log("Double Clicked");
            if (currentEvent == MouseEvent.Nothing || currentEvent == MouseEvent.Selection)
            {
                if (HitObject != null && Input.GetKey(KeyCode.LeftShift))
                {
                    Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

                    foreach (SelectableObject obj in AllObjects)
                    {
                        if (obj.type == HitObject.type &&
                            GeometryUtility.TestPlanesAABB(planes, obj.GetComponent<Renderer>().bounds) &&
                            !SelectedObjects.Contains(obj) && obj.gameObject.activeSelf) 
                        {
                            SelectedObjects.Add(obj);
                            currentEvent = MouseEvent.Selection;
                            obj.GetComponent<SelectableObject>().OnSelect();
                            selectionChanged = true;
                        }
                    }

                    SwitchPrimarySelected();
                }
            }
        }
    }
}