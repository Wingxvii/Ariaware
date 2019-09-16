using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvItemSlot<T> : InvItemType where T : Item
{
    JoinedVar<InvItemType, Item> itemSlot;
    public JoinedVar<InvItemType, Item> ItemSlot
    {
        get { return itemSlot; }
        protected set { itemSlot = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        ItemSlot = new JoinedVar<InvItemType, Item>(this, false);
        ItemBase = ItemSlot;
    }

    protected override void DeInitialize()
    {
        ItemSlot.Yeet();

        base.DeInitialize();
    }

    protected override void DestroyVars()
    {
        ItemSlot = null;

        base.DestroyVars();
    }
}
