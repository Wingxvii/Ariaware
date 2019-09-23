using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CentralManager : AbstractManager
{
    protected List<Scene> currentScene;

    List<InitObjectPool> disabledObjects;
    public List<InitObjectPool> DisabledObjects
    {
        get
        {
            if (disabledObjects == null)
                disabledObjects = new List<InitObjectPool>();
            return disabledObjects;
        }
        protected set { disabledObjects = value; }
    }

    List<Body> sceneBodies;
    public List<Body> SceneBodies
    {
        get { if (sceneBodies == null) { sceneBodies = new List<Body>(); } return sceneBodies; }
        protected set { sceneBodies = value; }
    }

    private CentralManager() { }

    static private CentralManager cM;
    static public CentralManager CM
    {
        get
        {
            if (cM == null)
                cM = new CentralManager();
            return cM;
        }
        set { cM = value; }
    }

    public bool CheckIfCreated()
    {
        if (cM == null)
            return false;
        return true;
    }

    public void DisableAndStore(InitializableObject io)
    {
        for (int i = DisabledObjects.Count - 1; i >= 0; --i)
        {
            if (DisabledObjects[i].sysType == io.GetType())
            {
                if (!DisabledObjects[i].ObjList.Contains(io))
                {
                    DisabledObjects[i].ObjList.Add(io);
                    io.enabled = false;
                }
                return;
            }
        }

        InitObjectPool newPool = new InitObjectPool(io.GetType());
        newPool.ObjList.Add(io);
        DisabledObjects.Add(newPool);
    }

    public void LoadAllSceneObjects(Scene objScene)
    {
        if (currentScene == null)
            currentScene = new List<Scene>();

        if (currentScene.Count == 0)
            currentScene.Add(objScene);
        else if (currentScene[0] == objScene)
            return;

        Debug.Log("ACTIVATE");

        SceneBodies.Clear();
        NetworkManager.NM.InitNetworking();

        currentScene[0] = objScene;
         
        InitializableObject[] io = Object.FindObjectsOfType<InitializableObject>();
        for (int i = io.Length - 1; i >= 0; --i)
        {
            io[i].Init();

            Body B = EType<Body>.FindType(io[i]);
            ProtectedAddBody(B);
        }

        for (int i = io.Length - 1; i >= 0; --i)
        {
            io[i].InnerInit();
        }

        //for (int i = io.Length - 1; i >= 0; --i)
        //{
        //    io[i].WireInit();
        //}
    }

    public void ProtectedAddBody(Body B)
    {
        if (B != null)
        {
            if (B.AE && !SceneBodies.Contains(B))
                SceneBodies.Add(B);
        }
    }

    public void RemoveBody(Body B)
    {
        SceneBodies.Remove(B);
    }
}