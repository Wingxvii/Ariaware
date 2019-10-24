using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermitFire : AbstractPermission<PermitFire, CommandFire, Gun, Controller>
{
    bool firing = false;

    protected override void HierarchyDeInitialize()
    {
        firing = false;

        base.HierarchyDeInitialize();
    }

    public void Fire(bool fire)
    {
        firing = fire;
    }

    protected override void FeedPuppet()
    {
        SpecificActor.FireBullet(firing);
    }
}
