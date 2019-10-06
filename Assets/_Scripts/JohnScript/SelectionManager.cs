using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MouseEvent
{
    Nothing = 0,
    Selection = 1,
    PrefabBuild = 2,
}


//SelectionManager.Instance.doSomething();

//void SaveToFile(Vector4(objType, x,y,z));
//Vector4(objType, x,y,z) LoadFromFile();

//SaveManager.Instacne.SaveToFile(fileData);



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

    public GameObject player;
    public bool boxActive = false;
    public Texture selectionBox;

    //mouse selection
    public MouseEvent currentEvent = MouseEvent.Nothing;

    //list of all objects
    public List<GameObject> AllObjects;
    //list of all selected objects
    public List<GameObject> SelectedObjects;

    public Vector3 mousePosition;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        SelectedObjects = new List<GameObject>();
        AllObjects.Add(player);
    }

    // Update is called once per frame
    void Update()
    {

        #region selection box
        if (boxActive && currentEvent == MouseEvent.PrefabBuild) {
            boxActive = false;
            boxStart = Vector2.zero;
            boxEnd = Vector2.zero;
        }

        if (Input.GetMouseButtonDown(0) && boxActive == false && currentEvent != MouseEvent.PrefabBuild)
        {
            boxStart = Input.mousePosition;
            boxActive = true;
        }
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
        if (Input.GetMouseButtonUp(0) && boxActive)
        {
            Vector3 worldSelection1;

            Ray rayCast = Camera.main.ScreenPointToRay(boxStart);
            RaycastHit castHit;
            if (Physics.Raycast(rayCast, out castHit, 500))
            {
                worldSelection1 = castHit.point;

                foreach (GameObject obj in AllObjects)
                {
                    if (obj.GetComponent<Transform>().position.x >= Mathf.Min(worldSelection1.x, mousePosition.x) &&
                        obj.GetComponent<Transform>().position.x <= Mathf.Max(worldSelection1.x, mousePosition.x) &&
                        obj.GetComponent<Transform>().position.z >= Mathf.Min(worldSelection1.z, mousePosition.z) &&
                        obj.GetComponent<Transform>().position.z <= Mathf.Max(worldSelection1.z, mousePosition.z) && !SelectedObjects.Contains(obj))
                    {
                        Debug.Log(obj.name);
                        SelectedObjects.Add(obj);
                        currentEvent = MouseEvent.Selection;
                        obj.GetComponent<SelectableObject>().OnSelect();

                    }
                }
            }

            boxStart = Vector2.zero;
            boxEnd = Vector2.zero;
            boxActive = false;
        }
        #endregion


        #region update mouse
        //update mouse position on screen
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 500) && hit.transform.gameObject.tag != "SelectableObject")
        {
            mousePosition = hit.point;
            //mousePosition = new Vector3(hit.point.x, 2, hit.point.z);
        }

        #endregion

        #region place prefab
        if (currentEvent == MouseEvent.PrefabBuild && Input.GetMouseButtonDown(0))
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                Object.Destroy(CommandPattern.Instance.prefabObject);
                ClearSelection();
            }
            CommandPattern.Instance.OnPlace(UseFactoryPattern(mousePosition, CommandPattern.Instance.prefabType));
        }
        else {

            #region selection
            //selection checking
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log(hit.transform.gameObject.name);

                if (hit.collider == null)
                {
                    foreach (GameObject obj in SelectedObjects)
                    {
                        obj.GetComponent<SelectableObject>().OnDeselect();
                    }
                    SelectedObjects.Clear();
                }
                else if (currentEvent == MouseEvent.PrefabBuild && Input.GetKey(KeyCode.LeftShift))
                {
                    //do nothing
                }
                else if (hit.collider != null && hit.transform.gameObject.tag == "SelectableObject")
                {
                    //deselect everything else if left control is not holded down
                    if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
                    {
                        ClearSelection();
                    }
                    SelectedObjects.Add(hit.transform.gameObject);
                    currentEvent = MouseEvent.Selection;
                    hit.transform.gameObject.GetComponent<SelectableObject>().OnSelect();
                }
                //deselect on ground hit
                else if (hit.collider != null && hit.transform.gameObject.tag == "Ground")
                {
                    foreach (GameObject obj in SelectedObjects)
                    {
                        obj.GetComponent<SelectableObject>().OnDeselect();
                    }
                    currentEvent = MouseEvent.Nothing;
                    SelectedObjects.Clear();
                }
            }
            else if (currentEvent == MouseEvent.PrefabBuild && Input.GetKeyUp(KeyCode.LeftShift))
            {
                Object.Destroy(CommandPattern.Instance.prefabObject);
                ClearSelection();
            }
            #endregion

        }

        #endregion




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
            foreach (GameObject obj in SelectedObjects)
            {
                obj.GetComponent<SelectableObject>().OnDeselect();
            }
            SelectedObjects.Clear();
        }
        currentEvent = MouseEvent.Nothing;
        boxStart = Vector2.zero;
        boxEnd = Vector2.zero;
        boxActive = false;
    }

    //here is the factory
    public GameObject UseFactoryPattern(Vector3 pos, BuildingEnum type) {

        GameObject returnObj;
        Building temp;
        switch (type)
        {

            case BuildingEnum.BlueBuilding:
                temp = new BlueBuilding(pos, out returnObj);
                returnObj.GetComponent<SelectableObject>().type = type;
                return returnObj;
            case BuildingEnum.RedBuilding:
                temp = new RedBuilding(pos, out returnObj);
                returnObj.GetComponent<SelectableObject>().type = type;
                return returnObj;
            case BuildingEnum.GreenBuilding:
                temp = new GreenBuilding(pos, out returnObj);
                returnObj.GetComponent<SelectableObject>().type = type;
                return returnObj;
            case BuildingEnum.YellowBuilding:
                temp = new YellowBuilding(pos, out returnObj);
                returnObj.GetComponent<SelectableObject>().type = type;
                return returnObj;
            default:
                return new GameObject();

        }

    }

    private void OnGUI()
    {
        if (boxStart != Vector2.zero && boxEnd != Vector2.zero) {
            GUI.DrawTexture(new Rect(boxStart.x, Screen.height - boxStart.y, boxEnd.x - boxStart.x, -1 * ((Screen.height - boxStart.y) - (Screen.height - boxEnd.y))), selectionBox);
        }
    }
}
