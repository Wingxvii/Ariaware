using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class BasePACES : InitializableObject
{
    public int ID;
    public bool sendData;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            return true;
        }

        return false;
    }

    protected override bool HierarchyInitialize()
    {
        if( base.HierarchyInitialize())
        {
            return true;
        }

        return false;
    }

    protected override bool CrossBranchInitialize()
    {
        if (base.CrossBranchInitialize())
        {
            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {


        base.CrossBranchDeInitialize();
    }


    protected override void HierarchyDeInitialize()
    {


        base.HierarchyDeInitialize();
    }

    protected override void InnerDeInitialize()
    {


        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {


        base.DestroyVars();
    }
}
