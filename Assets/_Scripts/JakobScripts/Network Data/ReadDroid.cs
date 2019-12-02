using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Body))]
public class ReadDroid : ReadBase
{
    Vector3 pseudoVel = Vector3.zero;
    Body b;
    public Animator anim;

    float dt = 0;
    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddThird();
            AddUpdate();

            b = GetComponent<Body>();
            anim = GetComponent<Animator>();

            return true;
        }

        return false;
    }

    protected override void UpdateObject()
    {
        //Debug.Log(b.Rb.velocity);
        anim.SetFloat("Walk", Vector3.Dot(pseudoVel, transform.forward) / 10);
        anim.SetFloat("Turn", Vector3.Dot(pseudoVel, transform.right) / 10);
    }

    protected override void Third()
    {
        dt += Time.fixedDeltaTime;
        if (NDM != null && NDM.Init())
        {
            int dID = b.ID - NET_PACKET.NetworkDataManager.FPSmax - 1;
            if (NET_PACKET.NetworkDataManager.ReadRTS.droidData[dID].flag)
            {

                NET_PACKET.NetworkDataManager.ReadRTS.droidData[dID].flag = false;

                pseudoVel = (NET_PACKET.NetworkDataManager.ReadRTS.droidData[dID].position - transform.position) / dt;

                transform.position = NET_PACKET.NetworkDataManager.ReadRTS.droidData[dID].position;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, NET_PACKET.NetworkDataManager.ReadRTS.droidData[dID].Yrot, transform.rotation.eulerAngles.z);
                dt = 0f;
            }
        }
    }
}
