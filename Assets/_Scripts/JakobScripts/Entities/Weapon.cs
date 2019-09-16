using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Item
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
}
