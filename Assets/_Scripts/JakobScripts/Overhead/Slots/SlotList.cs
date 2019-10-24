using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotList<T> : SlotBase where T : Entity
{
    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            EntityPlug = new JoinedList<SlotBase, Entity>(this);

            return true;
        }

        return false;
    }

    protected override bool HierarchyInitialize()
    {
        if (base.HierarchyInitialize())
        {
            if (Container.GetObj(0).BranchInit())
            {
                for (int i = Container.GetObj(0).AttachedEntities.Amount - 1; i >= 0; --i)
                {
                    Entity e = Container.GetObj(0).AttachedEntities.GetObj(i);
                    if (CullEntity(e) && e.BranchInit())
                    {
                        EntityPlug.Attach(e.AttachedSlot);
                    }
                }

                return true;
            }
        }

        return false;
    }

    public override bool CullEntity(Entity e)
    {
        return FType.FindIfType(e, typeof(T));
    }

    public override System.Type GetSlotType()
    {
        return typeof(T);
    }
}
