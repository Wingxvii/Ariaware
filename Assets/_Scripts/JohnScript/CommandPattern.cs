using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPattern : MonoBehaviour
{
    #region SingletonCode
    private static CommandPattern _instance;
    public static CommandPattern Instance { get { return _instance; } }
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
    public BuildingEnum prefabType;

    public GameObject redFab;
    public GameObject blueFab;
    public GameObject greenFab;
    public GameObject yellowFab;


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

    public void OnPrefabSelect(int prefab) {
        SelectionManager.Instance.OnPrefabCreation();
        Object.Destroy(prefabObject);

        switch (prefab) {
            case 1:
                prefabObject = (GameObject)Instantiate(redFab);
                prefabType = BuildingEnum.RedBuilding;
                break;
            case 2:
                prefabObject = (GameObject)Instantiate(greenFab);
                prefabType = BuildingEnum.GreenBuilding;
                break;
            case 3:
                prefabObject = (GameObject)Instantiate(blueFab);
                prefabType = BuildingEnum.BlueBuilding;
                break;
            case 4:
                prefabObject = (GameObject)Instantiate(yellowFab);
                prefabType = BuildingEnum.YellowBuilding;
                break;

        }
        //define the variable changes required for the prefab
        prefabObject.layer = 2;
        prefabObject.GetComponent<MeshCollider>().enabled = false;
        //prefabObject.GetComponent<SelectableObject>().enabled = false;
        prefabObject.SetActive(true);
    }


    // Start is called before the first frame update
    void Start()
    {
        prefabObject = (GameObject)Instantiate(redFab);
        prefabObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        #region hotkeys
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z)) {
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
        if (Input.GetKeyDown(KeyCode.I))
        {
            LoadMap();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveMap();
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
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            OnPrefabSelect(4);
        }

        #endregion

        if (prefabObject != null && prefabObject.activeSelf) {
            prefabObject.GetComponent<Transform>().position = new Vector3(SelectionManager.Instance.mousePosition.x, SelectionManager.Instance.mousePosition.y + prefabObject.GetComponent<Transform>().localScale.y/2.0f , SelectionManager.Instance.mousePosition.z);
            //prefabObject.GetComponent<Transform>().position = SelectionManager.Instance.mousePosition;
        }

    }

    public float Round(float num, float multiple)
    {
        int result = Mathf.RoundToInt(num / multiple);

        return result * multiple;
    }

    public void OnPlace(GameObject placeObject) {
        ClearCommands();
        _Undocommands.Push(new AddCommand(placeObject));
        SelectionManager.Instance.ClearSelection();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            SelectionManager.Instance.currentEvent = MouseEvent.PrefabBuild;
        }
    }

    public void OnUpgrade() {
        ClearCommands();
        foreach (GameObject obj in SelectionManager.Instance.SelectedObjects)
        {
            if (obj.GetComponent<SelectableObject>().level < 5)
            {
                _Undocommands.Push(new UpgradeCommand(obj));
            }
            else {
                Debug.Log("Object is already at Max Level");
            }
        }
    }

    public void OnDelete() {
        ClearCommands();
        foreach (GameObject obj in SelectionManager.Instance.SelectedObjects) {
            if (obj.GetComponent<SelectableObject>().deletable)
            {
                _Undocommands.Push(new DeleteCommand(obj));
            }
        }
        SelectionManager.Instance.ClearSelection();
    }

    public void OnDeleteAll()                                                               //LOOK HERE
    {
        ClearCommands();
        SelectionManager.Instance.ClearSelection();

        foreach (GameObject obj in SelectionManager.Instance.AllObjects)
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


        //SelectionManager.Instance.AllObjects.Clear();
    }

    public void ClearCommands() {
        foreach (ICommand command in _Redocommands)
        {
            command.Cleanup();
        }
        _Redocommands.Clear();
    }

    public void SaveMap()
    {
        MapLoader.clearFile();
        Debug.Log("Saving");
        foreach (GameObject obj in SelectionManager.Instance.AllObjects)
        {
            MapLoader.saveItem((int)obj.GetComponent<SelectableObject>().type, obj.GetComponent<Transform>().position.x, obj.GetComponent<Transform>().position.y, obj.GetComponent<Transform>().position.z);
        }
    }

    public void LoadMap()
    {
        OnDeleteAll();
        MapLoader.loadMap();
        Debug.Log("Loading...");
        int size = MapLoader.getObjectAmount();

        SelectionManager.Instance.player.GetComponent<Transform>().position = new Vector3(MapLoader.getX(0), MapLoader.getY(0), MapLoader.getZ(0));
        
        Debug.Log("Loading Buildings...");
        for(int i = 1; i < size; i++)
        {
            OnPlace(SelectionManager.Instance.UseFactoryPattern(new Vector3(MapLoader.getX(i), MapLoader.getY(i), MapLoader.getZ(i)), (BuildingEnum)MapLoader.getType(i)));
        }
    }
}
