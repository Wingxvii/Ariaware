using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class Item : Puppet
{
    JoinedVar<Item, Inventory> currentInventory;
    public JoinedVar<Item, Inventory> CurrentInventory
    {
        get { return currentInventory; }
        protected set { currentInventory = value; }
    }

    JoinedVar<Item, InvItemType> invType;
    public JoinedVar<Item, InvItemType> InvType
    {
        get { return invType; }
        protected set { invType = value; }

    }

    protected override void Initialize()
    {
        base.Initialize();

        AttachInventoryAndType();
    }

    public void AttachInventoryAndType()
    {
        bool hasInventory = AttachInventory();
        AttachInvType(hasInventory);
    }

    protected abstract InvItemType GetInv();

    protected bool AttachInventory()
    {
        Inventory inv = GetComponentInParent<Inventory>();
        if (inv != null && inv.AE)
        {
            inv.Init();
            inv.InnerInit();
            CurrentInventory.Attach(inv.Items);
            return true;
        }
        CurrentInventory.Yeet();
        return false;
    }

    protected bool AttachInvType(bool hasInventory)
    {
        if (hasInventory)
        {
            InvItemType itt = GetInv();
            if (itt != null && itt.AE)
            {
                itt.Init();
                itt.InnerInit();
                InvType.Attach(itt.ItemBase);
                return true;
            }
            InvType.Yeet(true);
            return false;
        }
        InvType.Yeet();
        return true;
    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();


    }

    protected override void CreateVars()
    {
        base.CreateVars();

        CurrentInventory = new JoinedVar<Item, Inventory>(this, false);
        InvType = new JoinedVar<Item, InvItemType>(this, true);
    }

    protected override void DeInitialize()
    {
        InvType.Yeet();
        CurrentInventory.Yeet();

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {


        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        InvType = null;
        CurrentInventory = null;

        base.DestroyVars();
    }

    protected override SlotBase GetSlot()
    {
        return Container.GetObj(0).GetComponent<ItemSlot>();
    }

    protected override bool OnReparent()
    {
        if (base.OnReparent())
        {
            bool hasInventory = AttachInventory();
            return AttachInvType(hasInventory);
        }
        return false;
    }
}
