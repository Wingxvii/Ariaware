using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using netcodeRTS;

public class RTSManager : MonoBehaviour
{
    #region SingletonCode
    private static RTSManager _instance;
    public static RTSManager Instance { get { return _instance; } }
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
    #endregion

    public GameObject prefabObject;
    public EntityType prefabType;

    public GameObject turretPrefab;
    public GameObject barracksPrefab;
    public GameObject wallPrefab;

    public GameObject moveCursorPrefab;
    public GameObject attackCursorPrefab;
    public GameObject rallyPrefab;

    //stack of undo and redo commands
    private Stack<ICommand> _Undocommands = new Stack<ICommand>();
    private Stack<ICommand> _Redocommands = new Stack<ICommand>();

    #region UndoRedo
    public void undo()
    {
        SelectionManager.Instance.ClearSelection();
        if (_Undocommands.Count != 0)
        {
            _Redocommands.Push(_Undocommands.Pop());
            _Redocommands.Peek().UnExecuteAction();
        }
    }
    public void redo()
    {
        SelectionManager.Instance.ClearSelection();
        if (_Redocommands.Count != 0)
        {
            _Undocommands.Push(_Redocommands.Pop());
            _Undocommands.Peek().ExecuteAction();
        }
    }
    #endregion

    public void OnPrefabSelect(int prefab)
    {
        SelectionManager.Instance.OnPrefabCreation();
        Object.Destroy(prefabObject);

        switch (prefab)
        {
            case 1:
                prefabObject = (GameObject)Instantiate(turretPrefab);
                prefabType = EntityType.Turret;
                break;
            case 2:
                prefabObject = (GameObject)Instantiate(barracksPrefab);
                prefabType = EntityType.Barracks;
                break;
            case 3:
                prefabObject = (GameObject)Instantiate(wallPrefab);
                prefabType = EntityType.Wall;
                break;
        }
        //define the variable changes required for the prefab
        prefabObject.GetComponent<Collider>().enabled = false;
        prefabObject.GetComponentInChildren<Canvas>().enabled = false;
        prefabObject.GetComponent<SelectableObject>().enabled = false;
        ((Behaviour)prefabObject.GetComponent("Halo")).enabled = false;
        if (prefabType == EntityType.Turret)
        {
            prefabObject.GetComponentInChildren<ParticleSystem>().Pause();
        }
        prefabObject.SetActive(true);
    }


    // Start is called before the first frame update
    void Start()
    {
        prefabObject = (GameObject)Instantiate(turretPrefab);
        prefabObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        #region hotkeys
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        {
            undo();
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y))
        {
            redo();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Break();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Delete))
        {
            OnDeleteAll();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            OnDelete();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            OnUpgrade();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnPrefabSelect(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnPrefabSelect(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnPrefabSelect(3);
        }
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }

        #endregion

        if (prefabObject != null && prefabObject.activeSelf)
        {
            prefabObject.GetComponent<Transform>().position = new Vector3(SelectionManager.Instance.mousePosition.x, SelectionManager.Instance.mousePosition.y + prefabObject.GetComponent<Transform>().localScale.y, SelectionManager.Instance.mousePosition.z);
            //prefabObject.GetComponent<Transform>().position = SelectionManager.Instance.mousePosition;
        }

    }

    public float Round(float num, float multiple)
    {
        int result = Mathf.RoundToInt(num / multiple);

        return result * multiple;
    }

    public void OnPlace(GameObject placeObject)
    {
        ClearCommands();
        _Undocommands.Push(new AddCommand(placeObject.GetComponent<SelectableObject>()));
        NetworkManager.SendBuildEntity(placeObject.GetComponent<SelectableObject>());

        SelectionManager.Instance.ClearSelection();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            SelectionManager.Instance.currentEvent = MouseEvent.PrefabBuild;
        }
    }

    public void OnUpgrade()
    {
        ClearCommands();
        foreach (SelectableObject obj in SelectionManager.Instance.SelectedObjects)
        {
            if (obj.level < 5)
            {
                _Undocommands.Push(new UpgradeCommand(obj));
            }
            else
            {
                Debug.Log("Object is already at Max Level");
            }
        }
    }

    public void OnDelete()
    {
        ClearCommands();
        foreach (SelectableObject obj in SelectionManager.Instance.SelectedObjects)
        {
            if (obj.destructable)
            {
                _Undocommands.Push(new DeleteCommand(obj));
            }
        }
        SelectionManager.Instance.ClearSelection();
    }

    public void OnDeleteAll()
    {
        ClearCommands();
        SelectionManager.Instance.ClearSelection();

        foreach (SelectableObject obj in SelectionManager.Instance.AllObjects)
        {
            SelectionManager.Instance.SelectedObjects.Add(obj);
        }

        OnDelete();

        //clear undo commands as well
        foreach (ICommand command in _Undocommands)
        {
            command.Cleanup();
        }
        _Undocommands.Clear();

    }

    public void ClearCommands()
    {
        foreach (ICommand command in _Redocommands)
        {
            command.Cleanup();
        }
        _Redocommands.Clear();
    }

    public void OnTrainBarracks(int unitType)
    {
        if (SelectionManager.Instance.PrimarySelectable.type == EntityType.Barracks)
        {
            foreach (SelectableObject obj in SelectionManager.Instance.SelectedObjects)
            {
                if (obj.type == EntityType.Barracks)
                {
                    switch (unitType)
                    {
                        case 1:
                            Barracks temp = (Barracks)obj;
                            temp.OnTrainRequest();
                            break;
                        default:
                            Debug.Log("ERROR UNIT TYPE MISSING");
                            break;
                    }
                }
            }
        }
    }

    public void OnSelectMove()
    {

        Object.Destroy(prefabObject);

        prefabObject = (GameObject)Instantiate(moveCursorPrefab);
        prefabObject.SetActive(true);

        SelectionManager.Instance.currentEvent = MouseEvent.UnitMove;

    }

    public void OnSelectAttack()
    {

        Object.Destroy(prefabObject);

        prefabObject = (GameObject)Instantiate(attackCursorPrefab);
        prefabObject.SetActive(true);

        SelectionManager.Instance.currentEvent = MouseEvent.UnitAttack;

    }

    public void OnReload()
    {
        foreach (SelectableObject obj in SelectionManager.Instance.SelectedObjects)
        {
            if (obj.type == EntityType.Turret)
            {
                Turret temp = (Turret)obj;
                temp.Reload();
            }
        }
    }
}