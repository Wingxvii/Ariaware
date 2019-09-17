using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public abstract class ObjectStat : InitializableObject
{
    JoinedVar<ObjectStat, Entity> ent;
    public JoinedVar<ObjectStat, Entity> Ent
    {
        get { return ent; }
        protected set { ent = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();

        
    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();

        Entity e = GetComponent<Entity>();
        e.Init();
        Ent.Attach(e.JoinedStats);
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        Ent = new JoinedVar<ObjectStat, Entity>(this, false);
    }

    protected override void DeInitialize()
    {
        //Ent.Yeet();

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        Ent.Yeet();

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        Ent = null;

        base.DestroyVars();
    }

    private void FixedUpdate()
    {
        UpdateData();
    }
    protected abstract void UpdateData();

    protected override void PostEnable()
    {
        base.PostEnable();

        if (Ent.GetObj(0) != null)
            Ent.GetObj(0).enabled = true;
    }
}
