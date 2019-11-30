using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BindVec3
{
    public bool x = false, y = false, z = false;
}

[RequireComponent(typeof(Puppet))]
public class ReadPlayerData : ReadBase
{
    Puppet p;

    public BindVec3 bindPos;
    public BindVec3 bindRot;
    public bool bindState = false;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddThird();

            p = GetComponent<Puppet>();

            return true;
        }

        return false;
    }

    protected override void DestroyVars()
    {
        p = null;

        base.DestroyVars();
    }

    protected override void Third()
    {
        Body b = EType<Body>.FindType(p);
        if (NDM != null && NDM.Init())
        {
            //Debug.Log(NET_PACKET.NetworkDataManager.ReadFPS.playerData[p.ID]);
            if (NET_PACKET.NetworkDataManager.ReadFPS.playerData[p.ID].flag)
            {
                Debug.Log("GETTING IT");
                NET_PACKET.NetworkDataManager.ReadFPS.playerData[p.ID].flag = false;

                transform.position = VectorSplit(NET_PACKET.NetworkDataManager.ReadFPS.playerData[p.ID].position, transform.position, bindPos);
                transform.localRotation = Quaternion.Euler(VectorSplit(NET_PACKET.NetworkDataManager.ReadFPS.playerData[p.ID].rotation, transform.localRotation.eulerAngles, bindRot));

                if (bindState && b != null)
                    if (b.Container.GetObj(0) != null)
                        b.Container.GetObj(0).pState = (uint)NET_PACKET.NetworkDataManager.ReadFPS.playerData[p.ID].state;
            }
        }
    }

    Vector3 VectorSplit(Vector3 v1, Vector3 v2, BindVec3 bv)
    {
        return new Vector3(bv.x ? v1.x : v2.x, bv.y ? v1.y : v2.y, bv.z ? v1.z : v2.z);
    }
}
