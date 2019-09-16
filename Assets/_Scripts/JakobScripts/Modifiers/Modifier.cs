using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public abstract class Modifier : InitializableObject
{
    JoinedVar<Modifier, Entity> baseEntity;
    public JoinedVar<Modifier, Entity> BaseEntity
    {
        get { return baseEntity; }
        protected set { baseEntity = value; }
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
        BaseEntity.Attach(e.Modifiers);
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        BaseEntity = new JoinedVar<Modifier, Entity>(this, false);
    }

    protected override void DeInitialize()
    {
        

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        BaseEntity.Yeet();

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        BaseEntity = null;

        base.DestroyVars();
    }

    protected override void PostEnable()
    {
        base.PostEnable();

        if (BaseEntity.GetObj(0) != null)
            BaseEntity.GetObj(0).enabled = true;
    }
}
