using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    public JoinedList<Gun, AmmoClip> ammo;
    public FireStats gunStats;
    public float FireDelay = 0.1f;
    float cooldown = 0f;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            ammo = new JoinedList<Gun, AmmoClip>(this);

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            AmmoClip[] ac = GetComponents<AmmoClip>();
            for (int i = 0; i < ac.Length; ++i)
            {
                if (ac[i].InnerInit())
                {
                    ammo.Attach(ac[i].gun);
                }
            }

            return true;
        }

        return false;
    }

    protected override void InnerDeInitialize()
    {
        ammo.Yeet();

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        ammo = null;

        base.DestroyVars();
    }

    public void FireBullet()
    {
        //Debug.Log("Firing");
        if (cooldown <= 0)
        {
            //Debug.Log("Firing!!!!");
            if (ammo.Amount > 0)
            {
                //Debug.Log("FIRING!!!!!!!!");
                float time = Time.fixedDeltaTime;
                Bullet b;
                while (time > FireDelay && ammo.GetObj(0).bulletCount > 0)
                {
                    time -= FireDelay;
                    ammo.GetObj(0).Shoot();
                    b = Instantiate(ammo.GetObj(0).bullet);
                    b.SetBulletStats(gunStats, transform.position, transform.rotation);
                }
                if (ammo.GetObj(0).bulletCount > 0)
                {
                    ammo.GetObj(0).Shoot();
                    b = Instantiate(ammo.GetObj(0).bullet);
                    b.SetBulletStats(gunStats, transform.position, transform.rotation);
                    cooldown = FireDelay;
                }
            }
        }
        if (cooldown > 0)
            cooldown -= Time.fixedDeltaTime;
    }
}
