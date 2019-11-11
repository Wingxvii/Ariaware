using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadDamageNPC : ReadBase
{
    //public Body b { get; protected set; }

    public Queue<int> Damages { get; protected set; }

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddThird();

            if (FPSManager.FM.NDM.Init())
                NET_PACKET.NetworkDataManager.damagesNPC.Add(this);

            Damages = new Queue<int>();

            //b = GetComponent<Body>();

            return true;
        }

        return false;
    }

    protected override void DestroyVars()
    {
        //b = null;

        base.DestroyVars();
    }

    protected override void Third()
    {
        if (NDM != null && NDM.Init())
        {
            NDM.ChugDamageNPCQueue();
        }

        while (Damages.Count > 0)
        {
            //b.Damage(Damages.Peek());
            Damages.Dequeue();
        }
    }
}
