using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Puppet
{
    JoinedList<Inventory, Item> items;
    public JoinedList<Inventory, Item> Items
    {
        get { return items; }
        protected set { items = value; }
    }

    JoinedList<Inventory, InvItemType> allowedItems;
    public JoinedList<Inventory, InvItemType> AllowedItems
    {
        get { return allowedItems; }
        protected set { allowedItems = value; }
    }

    JoinedVar<Inventory, VectorCursor> playerCursor;
    public JoinedVar<Inventory, VectorCursor> PlayerCursor
    {
        get { return playerCursor; }
        protected set { playerCursor = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();

        AttachCursor();

        AttachItems();
    }

    void AttachItems()
    {
        Item[] itm = GetComponentsInChildren<Item>();
        for (int i = itm.Length - 1; i >= 0; --i)
        {
            Inventory inv = itm[i].GetComponentInParent<Inventory>();
            if (inv == this)
            {
                if (itm[i].isActiveAndEnabled)
                {
                    itm[i].Init();
                    itm[i].InnerInit();
                    itm[i].AttachInventoryAndType();
                }
                //Items.Attach(itm[i].CurrentInventory);
            }
        }
    }

    void AttachCursor()
    {
        EntityContainer cont = Container.GetObj(0);
        if (cont != null && cont.isActiveAndEnabled)
        {
            VectorCursorSlot vcs = cont.GetComponent<VectorCursorSlot>();
            if (vcs != null && vcs.isActiveAndEnabled)
            {
                VectorCursor vc = EType<VectorCursor>.FindType(vcs.ObjectBase.GetObj(0));
                if (vc != null && vc.isActiveAndEnabled)
                {
                    vc.Init();
                    vc.InnerInit();
                    PlayerCursor.Attach(vc.Inventories);
                    return;
                }
            }
        }
        PlayerCursor.Yeet();
    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();

        InvItemType[] itt = GetComponents<InvItemType>();
        for (int i = itt.Length - 1; i >= 0; --i)
        {
            itt[i].Init();
            AllowedItems.Attach(itt[i].AttachedInventory);
        }
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        Items = new JoinedList<Inventory, Item>(this);
        AllowedItems = new JoinedList<Inventory, InvItemType>(this);
        PlayerCursor = new JoinedVar<Inventory, VectorCursor>(this, false);
    }

    protected override void DeInitialize()
    {
        Items.Yeet();
        PlayerCursor.Yeet();

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        AllowedItems.Yeet();

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        PlayerCursor = null;
        AllowedItems = null;
        Items = null;

        base.DestroyVars();
    }

    protected override SlotBase GetSlot()
    {
        return Container.GetObj(0).GetComponent<InventorySlot>();
    }

    protected override bool OnReparent()
    {
        if (base.OnReparent())
        {
            AttachCursor();

            return true;
        }
        return false;
    }

    protected override void PostDisable()
    {
        if (!enabled)
        {
            for (int i = AllowedItems.Joins.Count - 1; i >= 0; --i)
                AllowedItems.GetObj(i).enabled = false;
        }

        base.PostDisable();
    }
}
