using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvItemList<T> : InvItemType where T : Item
{
    JoinedList<InvItemType, Item> itemList;
    public JoinedList<InvItemType, Item> ItemList
    {
        get { return itemList; }
        protected set { itemList = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        ItemList = new JoinedList<InvItemType, Item>(this);
        ItemBase = ItemList;
    }

    protected override void DeInitialize()
    {
        ItemList.Yeet();

        base.DeInitialize();
    }

    protected override void DestroyVars()
    {
        ItemList = null;

        base.DestroyVars();
    }
}
