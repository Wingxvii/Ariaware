using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Body))]
public class ReadDroid : ReadBase
{
    Body b;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddThird();

            b = GetComponent<Body>();

            return true;
        }

        return false;
    }

    protected override void Third()
    {
        if (NDM != null && NDM.Init())
        {
            if (NET_PACKET.NetworkDataManager.ReadRTS.droidData[b.ID].flag)
            {

                NET_PACKET.NetworkDataManager.ReadRTS.droidData[b.ID].flag = false;

                transform.position = NET_PACKET.NetworkDataManager.ReadRTS.droidData[b.ID].position;
            }
        }
    }
}
