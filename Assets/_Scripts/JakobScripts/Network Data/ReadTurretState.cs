﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Puppet))]
public class ReadTurretState : ReadBase
{
    public TurretBullet TB;

    NET_PACKET.TurretStateData ts = NET_PACKET.TurretStateData.Idle;
    //public BindVec3 bindPos;
    public BindVec3 bindRot;
    public bool bindState;

    public float FireRate = 0.05f;
    float cooldown = 0f;
    Puppet p;

    int barrel = 0;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            p = GetComponent<Puppet>();

            AddFourth();
            AddUpdate();

            return true;
        }

        return false;
    }

    protected override void Fourth()
    {
        NDM.ChugTurrets();

        NET_PACKET.TurretSingle TUR = null;
        if (p.ID < NET_PACKET.NetworkDataManager.orderedTurretData.Count)
        {
            TUR = NET_PACKET.NetworkDataManager.orderedTurretData[p.ID];
        }

        if (TUR != null)
        {
            transform.rotation = Quaternion.Euler(VectorSplit(TUR.euler, transform.rotation.eulerAngles, bindRot));
            ts = (NET_PACKET.TurretStateData)TUR.state;
        }
        //else
        //{
        //    ts = NET_PACKET.TurretStateData.Idle;
        //}
    }

    protected override void UpdateObject()
    {
        if (ts == NET_PACKET.TurretStateData.IdleShooting || ts == NET_PACKET.TurretStateData.PositionalShooting || ts == NET_PACKET.TurretStateData.TargetedShooting || ts == NET_PACKET.TurretStateData.Recoil)
        {
            cooldown += Time.deltaTime;
            while (cooldown > FireRate)
            {
                if (TB != null)
                {
                    Instantiate(TB, transform.position, transform.rotation);
                }
         
                cooldown -= FireRate;
            }
        }
    }

    Vector3 VectorSplit(Vector3 v1, Vector3 v2, BindVec3 bv)
    {
        return new Vector3(bv.x ? v1.x : v2.x, bv.y ? v1.y : v2.y, bv.z ? v1.z : v2.z);
    }
}
