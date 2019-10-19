using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public abstract class InvItemType : Modifier
{
    public JoinedVar<InvItemType, Inventory> AttachedInventory;
    public Joined<InvItemType, Item> ItemBase;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AttachedInventory = new JoinedVar<InvItemType, Inventory>(this, false);

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

    protected override void HierarchyDeInitialize()
    {
        ItemBase.Yeet();

        base.HierarchyDeInitialize();
    }

    protected override void InnerDeInitialize()
    {
        AttachedInventory.Yeet();

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        ItemBase = null;
        AttachedInventory = null;

        base.DestroyVars();
    }

    protected void AttachInventory()
    {
        Inventory inv = GetComponent<Inventory>();
        if (inv.Init())
        {
            AttachedInventory.Attach(inv.AllowedItems);
        }
    }

    protected override bool PostEnable()
    {
        if (base.PostEnable())
        {
            if (!AttachedInventory.GetObj(0).enabled)
            {
                enabled = false;
                return false;
            }

            return true;
        }

        return false;
    }

    public abstract bool CullItem(Item i);

    public abstract System.Type GetItemType();
}
