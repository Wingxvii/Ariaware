using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Puppet
{
    public JoinedList<Inventory, Item> Items;

    public JoinedList<Inventory, InvItemType> AllowedItems;

    public JoinedVar<Inventory, VectorCursor> PlayerCursor;

    public JoinedVar<Inventory, Body> body;

    JoinedList<Inventory, InvStat> invStats;
    public JoinedList<Inventory, InvStat> InvStats
    {
        get { return invStats; }
        protected set { invStats = value; }
    }

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            Items = new JoinedList<Inventory, Item>(this);
            AllowedItems = new JoinedList<Inventory, InvItemType>(this);
            PlayerCursor = new JoinedVar<Inventory, VectorCursor>(this, false);
            body = new JoinedVar<Inventory, Body>(this);

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            InvItemType[] itt = GetComponents<InvItemType>();
            for (int i = itt.Length - 1; i >= 0; --i)
            {
                if (itt[i].InnerInit())
                {
                    AllowedItems.Attach(itt[i].AttachedInventory);
                }
            }

            return true;
        }

        return false;
    }

    protected override bool HierarchyInitialize()
    {
        if (base.HierarchyInitialize())
        {
            AttachItems();

            return true;
        }

        return false;
    }

    protected override bool CrossBranchInitialize()
    {
        if (base.CrossBranchInitialize())
        {
            AttachCursor();

            AttachBody();

            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {
        Items.Yeet();
        PlayerCursor.Yeet();

        base.HierarchyDeInitialize();
    }

    protected override void HierarchyDeInitialize()
    {
        base.HierarchyDeInitialize();
    }

    protected override void InnerDeInitialize()
    {
        AllowedItems.Yeet();

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        PlayerCursor = null;
        AllowedItems = null;
        Items = null;
        body = null;

        base.DestroyVars();
    }

    void AttachItems()
    {
        Item[] itm = GetComponentsInChildren<Item>();
        for (int i = itm.Length - 1; i >= 0; --i)
        {
            Inventory inv = itm[i].GetComponentInParent<Inventory>();
            if (inv == this)
            {
                if (itm[i].BranchInit())
                {
                    Items.Attach(itm[i].CurrentInventory);
                }
            }
        }
    }

    void AttachCursor()
    {
        EntityContainer cont = Container.GetObj(0);
        if (cont.TreeInit())
        {
            for (int i = 0; i < cont.AttachedSlots.Amount; ++i)
            {
                SlotBase sb = cont.AttachedSlots.GetObj(i);
                if (FType.FindIfType(sb.GetSlotType(), typeof(VectorCursorSlot)) && sb.BranchInit())
                {
                    for (int j = 0; j < sb.EntityPlug.Amount; ++j)
                    {
                        VectorCursor vc = EType<VectorCursor>.FindType(sb.EntityPlug.GetObj(j));
                        if (vc != null && vc.TreeInit())
                        {
                            PlayerCursor.Attach(vc.Inventories);
                            return;
                        }
                    }
                    return;
                }
            }
        }
    }

    void AttachBody()
    {
        EntityContainer cont = Container.GetObj(0);
        if (cont.TreeInit())
        {
            for (int i = 0; i < cont.AttachedSlots.Amount; ++i)
            {
                SlotBase sb = cont.AttachedSlots.GetObj(i);
                if (FType.FindIfType(sb.GetSlotType(), typeof(Body)) && sb.TreeInit())
                {
                    for (int j = 0; j < sb.EntityPlug.Amount; ++j)
                    {
                        Body b = EType<Body>.FindType(sb.EntityPlug.GetObj(j));
                        if (b != null && b.TreeInit())
                        {
                            body.Attach(b.inventories);
                            return;
                        }
                    }
                    return;
                }
            }
        }
    }

    protected override bool PostEnable()
    {
        if (base.PostEnable())
        {
            for (int i = 0; i < AllowedItems.Amount; i++)
            {
                AllowedItems.GetObj(i).AutoEnable();
            }

            return true;
        }

        return false;
    }

    protected override void PostDisable()
    {
        if (AC)
        {
            for (int i = 0; i < AllowedItems.Amount; i++)
            {
                AllowedItems.GetObj(i).AutoDisable();
            }
        }

        base.PostDisable();
    }
}
