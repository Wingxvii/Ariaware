using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using netcodeRTS;
public enum DroidType {
    Base,
}
public class DroidManager : MonoBehaviour
{
    #region SingletonCode
    private static DroidManager _instance;
    public static DroidManager Instance { get { return _instance; } }
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

    public List<Droid> Droidpool;
    public List<Droid> ActiveDroidPool;
    public GameObject PoolParent;

    //droid types
    public GameObject DroidPrefab;

    public float spawnRange;
    public float spawnHeight;

    public Player[] playerTargets;

    int fixedTimeStep;


    // Start is called before the first frame update
    void Start()
    {
        //CALL THIS DURING LOADING SCREEN
        InitPool();

        fixedTimeStep = (int)(1f / Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        #region Fixed Tick
        //count down
        --fixedTimeStep;

        //tick is called 10 times per 50 updates
        if (fixedTimeStep % 5 == 0)
        {
            TickUpdate();
        }

        //reset the clock
        if (fixedTimeStep <= 0)
        {
            //updates 50Hz
            fixedTimeStep = (int)(1f / Time.fixedDeltaTime);
        }
        #endregion

    }

    //called 10 times per second
    public void TickUpdate()
    {
        NetworkManager.SendDroidPositions();
    }



    //init a pool of droids to use
    void InitPool()
    {
        for (int counter = 0; counter < ResourceConstants.SUPPLY_MAX; counter++)
        {
            Droidpool.Add(GameObject.Instantiate(DroidPrefab, Vector3.zero, Quaternion.identity).GetComponent<Droid>());
        }

        foreach (Droid droid in Droidpool)
        {
            droid.gameObject.SetActive(false);
            droid.transform.parent = PoolParent.transform;
        }
    }


    //requests a drone to build, returns time to build
    public float RequestQueue(EntityType type)
    {
        if (ResourceManager.Instance.supplyCurrent < ResourceManager.Instance.totalSupply)
        {
            switch (type)
            {
                case EntityType.Droid:
                    ResourceManager.Instance.supplyCurrent++;
                    return 5f;
                default:
                    Debug.Log("ERROR: DROID TYPE INVALID");
                    return -1f;
            }
        }
        Debug.Log("MAX SUPPLY REACHED");
        return -1f;
    }

    //called when drone is requested to be built
    public void QueueFinished(Transform home, EntityType type)
    {
        switch (type)
        {
            case EntityType.Droid:
                //add offset here
                Vector3 pos = new Vector3(Random.Range(-1.0f, 1.0f), spawnHeight, Random.Range(-1.0f, 1.0f));
                pos = pos.normalized * spawnRange;

                SpawnDroid(type, new Vector3(home.position.x + pos.x, pos.y, home.position.z + pos.z));
                break;
            default:
                Debug.Log("ERROR: DROID TYPE INVALID");
                break;
        }
    }

    //called when drone is requested to be built, with a rally
    public void QueueFinished(Transform home, EntityType type, Vector3 rally)
    {
        switch (type)
        {
            case EntityType.Droid:
                //add offset here
                Vector3 pos = new Vector3(Random.Range(-1.0f, 1.0f), spawnHeight, Random.Range(-1.0f, 1.0f));
                pos = pos.normalized * spawnRange;

                SpawnDroid(type, new Vector3(home.position.x + pos.x, pos.y, home.position.z + pos.z));
                ActiveDroidPool[ActiveDroidPool.Count - 1].IssueLocation(rally);
                break;
            default:
                Debug.Log("ERROR: DROID TYPE INVALID");
                break;
        }
    }


    public void SpawnDroid(EntityType type, Vector3 pos) {
        Droidpool[Droidpool.Count - 1].transform.position = pos;
        Droidpool[Droidpool.Count - 1].gameObject.SetActive(true);

        NetworkManager.SendBuildEntity(Droidpool[Droidpool.Count - 1]);

        SelectionManager.Instance.AllObjects.Add(Droidpool[Droidpool.Count - 1]);
        ActiveDroidPool.Add(Droidpool[Droidpool.Count - 1]);

        Droidpool.RemoveAt(Droidpool.Count-1);
    }

    public void KillDroid(Droid droid) {
        ResourceManager.Instance.supplyCurrent--;
        
        Droidpool.Add(droid);
        droid.gameObject.SetActive(false);

        SelectionManager.Instance.AllObjects.Remove(droid);
        ActiveDroidPool.Remove(droid);
    }
}

