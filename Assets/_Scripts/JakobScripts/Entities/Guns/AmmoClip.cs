using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PACES.Gun))]
public class AmmoClip : ObjectStat
{
    public JoinedVar<AmmoClip, PACES.Gun> gun;
    public Bullet bullet;
    public int maxBulletCount = 6;
    public int bulletCount { get; protected set; }

    public void SetBullets(int num)
    {
        bulletCount = Mathf.Clamp(num, 0, maxBulletCount);
    }

    public void Shoot()
    {
        bulletCount = bulletCount > 0 ? bulletCount - 1 : 0;
    }

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            gun = new JoinedVar<AmmoClip, PACES.Gun>(this);

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            PACES.Gun g = GetComponent<PACES.Gun>();
            if (g.InnerInit())
            {
                gun.Attach(g.ammo);
                bulletCount = maxBulletCount;
            }

            return true;
        }

        return false;
    }

    protected override void InnerDeInitialize()
    {
        gun.Yeet();

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        gun = null;

        base.DestroyVars();
    }
}
