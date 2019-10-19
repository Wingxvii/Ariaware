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
                for (int i = AttachedInventory.GetObj(0).Items.Amount - 1; i >= 0; --i)
                {
                    Item it = AttachedInventory.GetObj(0).Items.GetObj(i);
                    if (CullItem(it) && it.BranchInit())
                    {
                        ItemBase.Attach(it.InvType);
                    }
                }

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
