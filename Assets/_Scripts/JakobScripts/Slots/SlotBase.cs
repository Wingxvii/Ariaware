using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityContainer))]
public abstract class SlotBase : InitializableObject
{
    JoinedVar<SlotBase, EntityContainer> container;
    public JoinedVar<SlotBase, EntityContainer> Container
    {
        get { return container; }
        protected set { container = value; }
    }

    Joined<SlotBase, Entity> objectBase;
    public Joined<SlotBase, Entity> ObjectBase
    {
        get { return objectBase; }
        protected set { objectBase = value; }
    }

    public void SetContainer()
    {
        EntityContainer ec = GetComponent<EntityContainer>();
        ec.Init();
        Container.Attach(ec.Slots);
    }

    protected override void Initialize()
    {
        base.Initialize();


    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();

        SetContainer();
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        Container = new JoinedVar<SlotBase, EntityContainer>(this, false);
    }

    protected override void DeInitialize()
    {


        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        Container.Yeet();

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        Container = null;

        base.DestroyVars();
    }

    protected override void PostEnable()
    {
        base.PostEnable();

        if (Container.GetObj(0) != null)
            Container.GetObj(0).enabled = true;
    }
}
