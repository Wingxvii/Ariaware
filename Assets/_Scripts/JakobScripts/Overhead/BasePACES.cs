﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class BasePACES : InitializableObject
{
    public int ID;
    public bool sendData;

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


    }

    protected override void DeInitialize()
    {


        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {


        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {


        base.DestroyVars();
    }
}