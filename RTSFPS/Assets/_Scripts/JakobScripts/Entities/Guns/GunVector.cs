using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunVector : Empty
{
    public JoinedVar<GunVector, Gun> parentGun;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            parentGun = new JoinedVar<GunVector, Gun>(this);

            return true;
        }

        return false;
    }

    protected override bool HierarchyInitialize()
    {
        if (base.HierarchyInitialize())
        {
            Gun[] guns = GetComponentsInParent<Gun>();
            for (int i = 0; i < guns.Length; i++)
            {
                if (guns[i].BranchInit())
                {
                    parentGun.Attach(guns[i].gunScope);
                    return true;
                }
            }
        }

        return false;
    }

    protected override void HierarchyDeInitialize()
    {
        parentGun.Yeet();

        base.HierarchyDeInitialize();
    }

    protected override void DestroyVars()
    {
        parentGun = null;

        base.DestroyVars();
    }
}
