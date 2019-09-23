﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InvStat : ObjectStat
{
    JoinedVar<InvStat, Inventory> attachedInventory;
    public JoinedVar<InvStat, Inventory> AttachedInventory
    {
        get { return attachedInventory; }
        protected set { attachedInventory = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();


    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();

        AttachInventory();
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        AttachedInventory = new JoinedVar<InvStat, Inventory>(this, false);
    }

    protected override void DeInitialize()
    {


        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        AttachedInventory.Yeet();

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        AttachedInventory = null;

        base.DestroyVars();
    }

    void AttachInventory()
    {
        Inventory inv = GetComponent<Inventory>();
        if (inv.AE)
        {
            inv.Init();

            AttachedInventory.Attach(inv.InvStats);
        }
    }
}
