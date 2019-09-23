using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class InitializableObject : MonoBehaviour
{
    protected float timer = 0f;
    public static float maxTimer = 1f;

    bool initialized = false;
    public bool Initialized
    {
        get { return initialized; }
        protected set { initialized = value; }
    }

    bool inner = false;
    public bool Inner
    {
        get { return inner; }
        protected set { inner = value; }
    }

    bool wired = false;
    public bool Wired
    {
        get { return wired; }
        protected set { wired = value; }
    }

    public bool AE
    {
        get { return gameObject.activeInHierarchy && enabled; }
    }

    public void Init()
    {
        if (!Initialized)
        {
            Initialized = true;

            CreateVars();
        }
    }

    public void DeInit()
    {
        if (Initialized)
        {
            DestroyVars();

            Initialized = false;
        }
    }

    public void InnerInit()
    {
        if (!Inner)
        {
            Inner = true;

            InnerInitialize();
        }
    }

    public void DeInnerInit()
    {
        if (Inner)
        {
            DeInnerInitialize();

            Inner = false;
        }
    }

    public void WireInit()
    {
        if (!Wired)
        {
            Wired = true;

            Initialize();
        }
    }

    public void WireDeInit()
    {
        if (Wired)
        {
            DeInitialize();

            Wired = false;
        }
    }

    protected virtual void Initialize()
    {
        
    }

    protected virtual void InnerInitialize()
    {

    }

    protected virtual void CreateVars()
    {

    }

    protected virtual void DeInitialize()
    {

    }

    protected virtual void DeInnerInitialize()
    {

    }

    protected virtual void DestroyVars()
    {

    }

    protected void OnEnable()
    {
        PostEnable();
    }

    protected virtual void PostEnable()
    {
        WireInit();
    }

    protected void OnDisable()
    {
        PostDisable();
    }

    protected virtual void PostDisable()
    {
        WireDeInit();
    }

    protected void Awake()
    {
        CentralManager.CM.LoadAllSceneObjects(gameObject.scene);

        Init();

        InnerInit();
    }

    protected void OnDestroy()
    {
        DeInnerInit();

        DeInit();
    }

    //private void OnTransformParentChanged()
    //{
    //    OnReparent();
    //}
    //
    //protected virtual void OnReparent()
    //{
    //
    //}

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > maxTimer)
        {
            SpitData();
            timer = 0;
        }
    }

    protected virtual void SpitData()
    {

    }
}
