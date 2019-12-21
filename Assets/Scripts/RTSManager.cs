using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using netcodeRTS;
using UnityEngine.SceneManagement;

namespace RTSManagers
{
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

        //used to get reference to current active blueprint
        public GameObject prefabObject;
        public EntityType prefabType;

        public GameObject turretBlueprint;
        public GameObject turretPrefab;
        public GameObject barracksBlueprint;
        public GameObject barracksPrefab;
        public GameObject wallBlueprint;
        public GameObject wallPrefab;
        public GameObject wallSideBlueprint;
        public GameObject wallSidePrefab;
        public GameObject moveCursorPrefab;
        public GameObject attackCursorPrefab;
        public GameObject rallyPrefab;

        public void OnPrefabSelect(int prefab)
        {
            SelectionManager.Instance.OnPrefabCreation();
            prefabObject.SetActive(false);

            switch (prefab)
            {
                case 1:
                    turretBlueprint.SetActive(true);
                    prefabObject = turretBlueprint;
                    prefabType = EntityType.Turret;
                    break;
                case 2:
                    barracksBlueprint.SetActive(true);
                    prefabObject = barracksBlueprint;
                    prefabType = EntityType.Barracks;
                    break;
                case 3:
                    wallBlueprint.SetActive(true);
                    prefabObject = wallBlueprint;
                    prefabType = EntityType.Wall;
                    break;
                case 4:
                    wallSideBlueprint.SetActive(true);
                    prefabObject = wallSideBlueprint;
                    prefabType = EntityType.Wall;
                    break;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            //set this as active scene
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(2));


            //prefab instanitation
            turretBlueprint = Instantiate(turretBlueprint);
            turretBlueprint.SetActive(false);
            barracksBlueprint = Instantiate(barracksBlueprint);
            barracksBlueprint.SetActive(false);
            wallBlueprint = Instantiate(wallBlueprint);
            wallBlueprint.SetActive(false);
            moveCursorPrefab = Instantiate(moveCursorPrefab);
            moveCursorPrefab.SetActive(false);
            attackCursorPrefab = Instantiate(attackCursorPrefab);
            attackCursorPrefab.SetActive(false);
            rallyPrefab = Instantiate(rallyPrefab);
            rallyPrefab.SetActive(false);
            prefabObject = turretBlueprint;
            prefabObject.SetActive(false);


            dll.UserMetrics.ClearFile();
            dll.UserMetrics.Reset();
        }

        // Update is called once per frame
        void Update()
        {
            #region hotkeys
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
                //OnUpgrade();
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
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                OnPrefabSelect(4);
            }
            if (Input.GetKeyDown("escape"))
            {
                Application.Quit();
            }
            if (Input.GetKeyDown(KeyCode.T)) { 
                foreach(Droid droid in DroidManager.Instance.ActiveDroidPool)
                {
                    droid.OnDeath();
                }
            }

            dll.UserMetrics.UpdateFile();


            #endregion

            //bind prefab object to mouse
            if (prefabObject != null && prefabObject.activeSelf)
            {
                prefabObject.GetComponent<Transform>().position = new Vector3(SelectionManager.Instance.mousePosition.x, SelectionManager.Instance.mousePosition.y + prefabObject.GetComponent<Transform>().localScale.y, SelectionManager.Instance.mousePosition.z);
            }

        }

        public void OnPlace(GameObject placeObject)
        {
            placeObject.gameObject.SetActive(true);
            placeObject.GetComponent<SelectableObject>().OnActivation();
        }

        /*
        public void OnUpgrade()
        {
            foreach (SelectableObject obj in SelectionManager.Instance.SelectedObjects)
            {
                obj.level++;
                Transform objTransf = obj.GetComponent<Transform>();

                //make it bigger
                objTransf.localScale = new Vector3(objTransf.localScale.x * 1.1f, objTransf.localScale.y * 1.1f, objTransf.localScale.z * 1.1f);
                //make sure its anchored on the ground
                objTransf.position = new Vector3(objTransf.position.x, 0 + objTransf.localScale.y * 0.5f, objTransf.position.z);
            }
        }
        */

        public void OnDelete()
        {
            foreach (SelectableObject obj in SelectionManager.Instance.SelectedObjects)
            {
                obj.OnDeactivation();
                obj.gameObject.SetActive(false);
                SelectionManager.Instance.AllObjects.Remove(obj);
            }
            SelectionManager.Instance.ClearSelection();
        }

        public void OnDeleteAll()
        {
            SelectionManager.Instance.ClearSelection();

            foreach (SelectableObject obj in SelectionManager.Instance.AllObjects)
            {
                SelectionManager.Instance.SelectedObjects.Add(obj);
            }

            OnDelete();
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
            prefabObject.SetActive(false);
            prefabObject = moveCursorPrefab;
            prefabObject.SetActive(true);

            SelectionManager.Instance.currentEvent = MouseEvent.UnitMove;

        }

        public void OnSelectAttack()
        {

            prefabObject.SetActive(false);
            prefabObject = attackCursorPrefab;
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

        public void OnRally()
        {
            prefabObject.SetActive(false);
            prefabObject = rallyPrefab;
            prefabObject.SetActive(true);

            SelectionManager.Instance.currentEvent = MouseEvent.Rally;

        }

    }
}