using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RTSFactory))]
public class ReadKilled : ReadBase
{
    RTSFactory RTSF;
    public Queue<int> Kills { get; protected set; }

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddFirst();

            if (FPSManager.FM.NDM.Init())
                NET_PACKET.NetworkDataManager.kills.Add(this);

            RTSF = GetComponent<RTSFactory>();

            Kills = new Queue<int>();

            return true;
        }

        return false;
    }

    protected override void DestroyVars()
    {


        base.DestroyVars();
    }

    protected override void First()
    {
        if (NDM != null && NDM.Init())
        {
            NDM.ChugKillQueue();
        }

        while (Kills.Count > 0)
        {
            RTSF.NukeContainer(Kills.Peek());
            Kills.Dequeue();
        }
    }
}
