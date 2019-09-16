using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public abstract class InvItemType : Modifier
{
    JoinedVar<InvItemType, Inventory> attachedInventory;
    public JoinedVar<InvItemType, Inventory> AttachedInventory
    {
        get { return attachedInventory; }
        protected set { attachedInventory = value; }
    }

    Joined<InvItemType, Item> itemBase;
    public Joined<InvItemType, Item> ItemBase
    {
        get { return itemBase; }
        protected set { itemBase = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected void AttachInventory()
    {
        AttachedInventory.Yeet();
        Inventory inv = GetComponent<Inventory>();
        inv.Init();
        AttachedInventory.Attach(inv.AllowedItems);
    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();

        AttachInventory();
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        AttachedInventory = new JoinedVar<InvItemType, Inventory>(this, false);
    }

    protected override void DeInitialize()
    {
        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        AttachedInventory.Yeet();

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        ItemBase = null;
        AttachedInventory = null;

        base.DestroyVars();
    }

    protected override void PostEnable()
    {
        base.PostEnable();

        if (AttachedInventory.GetObj(0) != null)
            AttachedInventory.GetObj(0).enabled = true;
    }
}
