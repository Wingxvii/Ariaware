using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class InitializableObject : MonoBehaviour
{
    protected float timer = 0f;
    public static float maxTimer = 1f;

    public bool created = false;
    public bool attemptedCreated { get; set; } = false;

    public bool inner = false;
    public bool attemptedInner { get; set; } = false;

    public bool contain = false;
    public bool attemptedContain { get; set; } = false;

    public bool wired = false;
    public bool attemptedWired { get; set; } = false;

    public bool autoDisabled { get; private set; } = false;

    public bool AE
    {
        get { return gameObject.activeInHierarchy && enabled; }
    }

    public bool AC
    {
        get { return gameObject.activeInHierarchy; }
    }

    public bool EN
    {
        get { return enabled; }
    }

    public bool Init()
    {
        attemptedCreated = true;

        if (!created && AC)
        {
            created = true;

            if (!CreateVars())
            {
                DeInit();
            }
        }

        return created;
    }

    public void DeInit()
    {
        attemptedCreated = false;

        if (created)
        {
            InnerDeInit();

            DestroyVars();

            created = false;
        }
    }

    public bool InnerInit()
    {
        attemptedInner = true;

        if (!inner && Init())
        {
            inner = true;
            if (!InnerInitialize())
            {
                InnerDeInit();
            }
        }

        return inner;
    }

    public void InnerDeInit()
    {
        attemptedInner = false;

        if (inner)
        {
            BranchDeInit();

            InnerDeInitialize();

            inner = false;
        }
    }

    public bool BranchInit()
    {
        attemptedContain = true;

        if (!contain && AE && InnerInit())
        {
            contain = true;

            if (!HierarchyInitialize())
            {
                BranchDeInit();
            }
        }

        return contain;
    }

    public void BranchDeInit()
    {
        attemptedContain = false;

        if (contain)
        {
            TreeDeInit();

            HierarchyDeInitialize();

            contain = false;
        }
    }

    public bool TreeInit()
    {
        attemptedWired = true;

        if (!wired && BranchInit())
        {
            wired = true;

            if (!CrossBranchInitialize())
            {
                TreeDeInit();
            }
        }

        return wired;
    }

    public void TreeDeInit()
    {
        attemptedWired = false;

        if (wired)
        {
            CrossBranchDeInitialize();

            wired = false;
        }
    }

    protected virtual bool CreateVars()
    {
        return true;
    }

    protected virtual bool InnerInitialize()
    {
        return true;
    }

    protected virtual bool HierarchyInitialize()
    {
        return true;
    }

    protected virtual bool CrossBranchInitialize()
    {
        return true;
    }

    protected virtual void CrossBranchDeInitialize()
    {
        
    }

    protected virtual void HierarchyDeInitialize()
    {
        
    }

    protected virtual void InnerDeInitialize()
    {
        
    }

    protected virtual void DestroyVars()
    {
        
    }

    protected void OnEnable()
    {
        PostEnable();
    }

    protected virtual bool PostEnable()
    {
        //BranchInit();

        return TreeInit();
    }

    protected void OnDisable()
    {
        PostDisable();
    }

    protected virtual void PostDisable()
    {
        //TreeDeInit();

        BranchDeInit();
    }

    protected void Awake()
    {
        //CentralManager.CM.LoadAllSceneObjects(gameObject.scene);

        //Init();

        InnerInit();
        //TreeInit();
    }

    protected void OnDestroy()
    {
        //InnerDeInit();

        DeInit();
    }

    public void AutoDisable()
    {
        if (enabled & !autoDisabled)
        {
            autoDisabled = true;
            enabled = false;
        }
    }

    public void AutoEnable()
    {
        if (!enabled && autoDisabled)
        {
            autoDisabled = false;
            enabled = true;
        }
    }

    //private void Update()
    //{
    //    timer += Time.deltaTime;
    //    if (timer > maxTimer)
    //    {
    //        SpitData();
    //        timer = 0;
    //    }
    //}

    protected virtual void SpitData()
    {

    }
}
