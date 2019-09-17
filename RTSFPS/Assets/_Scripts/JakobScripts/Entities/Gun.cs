using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
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

    protected override InvItemType GetInv()
    {
        return CurrentInventory.GetObj(0).GetComponent<InvGuns>();
    }
}
