using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Puppet))]
public class ReadTurretState : ReadBase
{
    TurretState ts = TurretState.Idle;
    //public BindVec3 bindPos;
    public BindVec3 bindRot;
    public bool bindState;

    Puppet p;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            p = GetComponent<Puppet>();

            AddFourth();

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
            ts = (TurretState)TUR.state;
        }
        else
        {
            ts = TurretState.Idle;
        }
    }

    Vector3 VectorSplit(Vector3 v1, Vector3 v2, BindVec3 bv)
    {
        return new Vector3(bv.x ? v1.x : v2.x, bv.y ? v1.y : v2.y, bv.z ? v1.z : v2.z);
    }
}
