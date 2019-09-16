using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Slot<T> : SlotBase where T : Entity
{
    JoinedVar<SlotBase, Entity> objectSlot;
    public JoinedVar<SlotBase, Entity> ObjectSlot
    {
        get { return objectSlot; }
        private set { objectSlot = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();


    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();


    }

    protected override void CreateVars()
    {
        base.CreateVars();

        ObjectSlot = new JoinedVar<SlotBase, Entity>(this, false);
        ObjectBase = ObjectSlot;
    }

    protected override void DeInitialize()
    {
        ObjectSlot.Yeet();

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        ObjectSlot = null;

        base.DestroyVars();
    }
}
