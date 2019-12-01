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
            int dID = b.ID - NET_PACKET.NetworkDataManager.FPSmax - 1;
            if (NET_PACKET.NetworkDataManager.ReadRTS.droidData[dID].flag)
            {

                NET_PACKET.NetworkDataManager.ReadRTS.droidData[dID].flag = false;

                transform.position = NET_PACKET.NetworkDataManager.ReadRTS.droidData[dID].position;
            }
        }
    }
}
