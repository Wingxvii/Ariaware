using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermitInvCycle : AbsInvPermission<PermitInvCycle, CommandInvCycle, Inventory, Controller>
{
    int invSelected = 0;
    int preSelected = 0;
    public float timeDelay = 0f;
    float timeRem = 0f;

    JoinedVar<PermitInvCycle, Item> attachedItem;
    public JoinedVar<PermitInvCycle, Item> AttachedItem
    {
        get { return attachedItem; }
        protected set { attachedItem = value; }
    }

    void SetAttachedItem()
    {
        for (int i = SpecificActor.Items.Joins.Count - 1; i >= 0; --i)
        {
            
        }
    }

    protected override void Initialize()
    {
        base.Initialize();


    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        AttachedItem = new JoinedVar<PermitInvCycle, Item>(this, false);
    }

    protected override void DeInitialize()
    {
        AttachedItem.Yeet();

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        AttachedItem = null;

        base.DestroyVars();
    }

    public void ReceiveInput(int axisValue)
    {
        if (timeRem <= 0f)
        {
            invSelected += axisValue;
            timeRem = timeDelay;
        }
        else
        {
            timeRem -= Time.deltaTime;
        }
    }

    protected override void FeedPuppet()
    {
        Bound();

        if (preSelected != invSelected)
        {
            
        }
    }

    public void Bound()
    {
        if (SpecificActor.Items.Joins.Count > 0)
        {
            if (invSelected < 0)
                invSelected = 0;
            else if (invSelected > SpecificActor.Items.Joins.Count - 1)
                invSelected = SpecificActor.Items.Joins.Count - 1;
        }
    }
}
