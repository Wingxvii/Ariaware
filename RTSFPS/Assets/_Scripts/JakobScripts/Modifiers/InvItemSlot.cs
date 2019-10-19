using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InvItemSlot<T> : InvItemType where T : Item
{
    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            ItemBase = new JoinedVar<InvItemType, Item>(this);

            return true;
        }

        return false;
    }

    protected override bool HierarchyInitialize()
    {
        if (base.HierarchyInitialize())
        {
            if (AttachedInventory.GetObj(0).BranchInit())
            {

                return true;
            }
        }

        return false;
    }

    public override bool CullItem(Item i)
    {
        return FType.FindIfType(i, typeof(T));
    }

    public override System.Type GetItemType()
    {
        return typeof(T);
    }
}
