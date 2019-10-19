using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorCursor : Puppet
{
    public float magnitude;
    public JoinedList<VectorCursor, Inventory> Inventories;

    protected override bool CrossBranchInitialize()
    {
        if (base.CrossBranchInitialize())
        {
            AttachInventories();

            return true;
        }

        return false;
    }

    void AttachInventories()
    {
        EntityContainer ec = Container.GetObj(0);
        if (ec.TreeInit())
        {
            for (int i = 0; i < ec.AttachedSlots.Amount; ++i)
            {
                SlotBase sb = ec.AttachedSlots.GetObj(i);
                if (FType.FindIfType(sb.GetSlotType(), typeof(Inventory)) && sb.BranchInit())
                {
                    for (int j = 0; j < sb.EntityPlug.Amount; ++j)
                    {
                        Inventory inv = EType<Inventory>.FindType(sb.EntityPlug.GetObj(j));
                        if (inv != null && inv.TreeInit())
                        {
                            Inventories.Attach(inv.PlayerCursor);
                        }
                    }
                }
            }
        }
    }

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            Inventories = new JoinedList<VectorCursor, Inventory>(this);

            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {
        Inventories.Yeet();

        base.CrossBranchDeInitialize();
    }

    protected override void DestroyVars()
    {
        Inventories = null;

        base.DestroyVars();
    }
}
