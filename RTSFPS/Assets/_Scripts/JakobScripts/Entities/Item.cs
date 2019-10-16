using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class Item : Puppet
{
    public JoinedVar<Item, Inventory> CurrentInventory;
    public JoinedVar<Item, InvItemType> InvType;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            CurrentInventory = new JoinedVar<Item, Inventory>(this, false);
            InvType = new JoinedVar<Item, InvItemType>(this, true);

            return true;
        }

        return false;
    }

    protected override bool HierarchyInitialize()
    {
        if (base.HierarchyInitialize())
        {
            if (!AttachInventory())
                return false;

            if (!AttachInvType())
                return false;

            return true;
        }

        return false;
    }

    

    protected override void HierarchyDeInitialize()
    {
        InvType.Yeet();
        CurrentInventory.Yeet();

        base.HierarchyDeInitialize();
    }

    protected override void DestroyVars()
    {
        InvType = null;
        CurrentInventory = null;

        base.DestroyVars();
    }

    protected bool AttachInventory()
    {
        Inventory[] inv = GetComponentsInParent<Inventory>();
        if (inv.Length > 0)
        {
            for (int i = 0; i < inv.Length; ++i)
            {
                if (inv[i].BranchInit())
                {
                    CurrentInventory.Attach(inv[i].Items);
                    return true;
                }
            }

            CurrentInventory.Yeet(true);
        }

        return false;
    }

    protected bool AttachInvType()
    {
        for (int i = CurrentInventory.GetObj(0).AllowedItems.Amount - 1; i >= 0; --i)
        {
            InvItemType itt = CurrentInventory.GetObj(0).AllowedItems.GetObj(i);
            if (itt.BranchInit() && itt.CullItem(this))
            {
                InvType.Attach(itt.ItemBase);
                return true;
            }
        }

        Debug.Log("OUTTA HERE");

        InvType.Yeet(true);

        return false;
    }
}
