using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : Puppet
{
    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void CreateVars()
    {
        base.CreateVars();
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
        return Container.GetObj(0).GetComponent<RagdollSlot>();
    }
}
