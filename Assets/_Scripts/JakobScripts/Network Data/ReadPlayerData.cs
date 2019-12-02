using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BindVec3
{
    public BindVec3() { }
    public BindVec3(bool X, bool Y, bool Z) { x = X; y = Y; z = Z; }

    public bool x = false, y = false, z = false;
}

[RequireComponent(typeof(Puppet))]
public class ReadPlayerData : ReadBase
{
    Puppet p;
    Animator anim;
    Vector3 pseudoVel = Vector3.zero;
    float dt = 0;

    public BindVec3 bindPos;
    public BindVec3 bindRot;
    public bool bindState = false;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddThird();
            AddUpdate();

            p = GetComponent<Puppet>();
            anim = GetComponent<Animator>();

            return true;
        }

        return false;
    }

    protected override void DestroyVars()
    {
        p = null;

        base.DestroyVars();
    }

    protected override void UpdateObject()
    {
        if (anim != null)
        {
            anim.SetFloat("Walk", Vector3.Dot(pseudoVel, transform.forward) / 10);
            anim.SetFloat("Turn", Vector3.Dot(pseudoVel, transform.right) / 10);
        }
    }

    protected override void Third()
    {
        dt += Time.deltaTime;
        Body b = EType<Body>.FindType(p);
        if (NDM != null && NDM.Init())
        {
            //Debug.Log(NET_PACKET.NetworkDataManager.ReadFPS.playerData[p.ID]);
            if (NET_PACKET.NetworkDataManager.ReadFPS.playerData[p.ID].flag)
            {
                //Debug.Log("GETTING IT");
                //NET_PACKET.NetworkDataManager.ReadFPS.playerData[p.ID].flag = false;

                pseudoVel = (NET_PACKET.NetworkDataManager.ReadFPS.playerData[p.ID].position - transform.position) / dt;
                dt = 0f;

                transform.position = VectorSplit(NET_PACKET.NetworkDataManager.ReadFPS.playerData[p.ID].position, transform.position, bindPos);
                transform.localRotation = Quaternion.Euler(VectorSplit(NET_PACKET.NetworkDataManager.ReadFPS.playerData[p.ID].rotation, transform.localRotation.eulerAngles, bindRot));

                if (bindState && b != null)
                {
                    if (b.Container.GetObj(0) != null)
                        b.Container.GetObj(0).pState = (uint)NET_PACKET.NetworkDataManager.ReadFPS.playerData[p.ID].state;
                    if (anim != null)
                    {
                        if ((b.Container.GetObj(0).pState | (uint)PlayerState.Jumping) > 0)
                        {
                            anim.Play("Jump");
                        }
                    }
                }
                //Debug.Log(name);
            }
        }
    }

    Vector3 VectorSplit(Vector3 v1, Vector3 v2, BindVec3 bv)
    {
        return new Vector3(bv.x ? v1.x : v2.x, bv.y ? v1.y : v2.y, bv.z ? v1.z : v2.z);
    }
}
