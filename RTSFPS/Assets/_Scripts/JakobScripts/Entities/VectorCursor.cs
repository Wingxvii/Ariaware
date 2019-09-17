using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorCursor : Puppet
{
    public float magnitude;

    JoinedList<VectorCursor, Inventory> inventories;
    public JoinedList<VectorCursor, Inventory> Inventories
    {
        get { return inventories; }
        protected set { inventories = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();

        AttachInventories();
    }

    void AttachInventories()
    {
        Inventories.Yeet();
        EntityContainer ec = Container.GetObj(0);
        if (ec != null && ec.isActiveAndEnabled)
        {
            InventorySlot invs = ec.GetComponent<InventorySlot>();
            if (invs != null && invs.isActiveAndEnabled)
            {
                Inventory inv;
                for (int i = invs.ObjectList.Joins.Count - 1; i >= 0; --i)
                {
                    inv = EType<Inventory>.FindType(invs.ObjectList.GetObj(i));
                    if (inv != null && inv.isActiveAndEnabled)
                    {
                        inv.Init();
                        inv.InnerInit();
                        Inventories.Attach(inv.PlayerCursor);
                    }
                }
            }
        }
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        Inventories = new JoinedList<VectorCursor, Inventory>(this);
    }

    protected override void DeInitialize()
    {


        base.DeInitialize();
    }

    protected override void DestroyVars()
    {


        base.DestroyVars();
    }

    protected override SlotBase GetSlot()
    {
        return Container.GetObj(0).GetComponent<VectorCursorSlot>();
    }

    protected override bool OnReparent()
    {
        if (base.OnReparent()) 
        {
            AttachInventories();

            return true;
        }
        return false;
    }
}
