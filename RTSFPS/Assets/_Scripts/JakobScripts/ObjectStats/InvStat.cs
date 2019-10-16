using System.Collections;
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

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AttachedInventory = new JoinedVar<InvStat, Inventory>(this, false);

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            AttachInventory();

            return true;
        }

        return false;
    }

    protected override void InnerDeInitialize()
    {
        AttachedInventory.Yeet();

        base.InnerDeInitialize();
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
