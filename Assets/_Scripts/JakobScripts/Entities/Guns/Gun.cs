﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PACES
{
    public class Gun : Weapon
    {
        public JoinedList<Gun, AmmoClip> ammo;
        public JoinedList<Gun, GunVector> gunScope;
        public FireStats gunStats;
        public float FireDelay = 0.1f;
        float cooldown = 0f;

        public AnimationCurve accuracyCalc;

        protected override bool CreateVars()
        {
            if (base.CreateVars())
            {
                gunScope = new JoinedList<Gun, GunVector>(this);
                ammo = new JoinedList<Gun, AmmoClip>(this);

                AddFixedUpdate();

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

        protected override bool HierarchyInitialize()
        {
            if (base.HierarchyInitialize())
            {
                GunVector[] gv = GetComponentsInChildren<GunVector>();
                for (int i = 0; i < gv.Length; i++)
                {
                    Gun gvGun = gv[i].GetComponentInParent<Gun>();
                    if (gvGun == this)
                    {
                        if (gv[i].BranchInit())
                        {
                            gunScope.Attach(gv[i].parentGun);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        protected override void HierarchyDeInitialize()
        {
            gunScope.Yeet();

            base.HierarchyDeInitialize();
        }

        protected override void InnerDeInitialize()
        {
            ammo.Yeet();

            base.InnerDeInitialize();
        }

        protected override void DestroyVars()
        {
            gunScope = null;
            ammo = null;

            base.DestroyVars();
        }

        protected override void FixedUpdateObject()
        {
            //if (b != null && !b.notYourBody)
            //    Debug.Log(Container.GetObj(0).pState & (uint)PlayerState.Shooting);
            if (b != null && b.notYourBody)
            {
                Inventory inv = CurrentInventory.GetObj(0);
                if (inv.Items.GetObj(inv.activeObject) == this)
                {
                    //Debug.Log(name);
                    FireBullet((Container.GetObj(0).pState & (uint)PlayerState.Shooting) > 0);
                }
            }
            
        }

        public void FireBullet(bool canFire)
        {
            //Debug.Log("Firing");
            if (canFire && cooldown <= 0)
            {
                //Debug.Log("Firing!!!!");
                if (ammo.Amount > 0)
                {
                    //saveShooting |= (uint)PlayerState.Shooting;
                    //Debug.Log("FIRING!!!!!!!!");
                    float time = Time.fixedDeltaTime;
                    Bullet b = null;
                    while (time > FireDelay && ammo.GetObj(0).bulletCount > 0)
                    {
                        time -= FireDelay;
                        ShootBullets(b);
                    }
                    if (ammo.GetObj(0).bulletCount > 0)
                    {
                        ShootBullets(b);

                        cooldown = FireDelay;
                    }
                }
            }
            if (cooldown > 0)
                cooldown -= Time.fixedDeltaTime;

            if (!b.notYourBody)
            {
                if (canFire)
                    Container.GetObj(0).pState |= (uint)PlayerState.Shooting;
                else
                    Container.GetObj(0).pState &= ~(uint)PlayerState.Shooting;
            }
        }

        void ShootBullets(Bullet b)
        {
            for (int i = 0; i < gunScope.Amount; ++i)
            {
                ammo.GetObj(0).Shoot();
                b = Instantiate(ammo.GetObj(0).bullet);
                b.SetBulletStats(gunStats, gunScope.GetObj(i), CurrentInventory.GetObj(0).body.GetObj(0), accuracyCalc);
            }
        }
    }
}