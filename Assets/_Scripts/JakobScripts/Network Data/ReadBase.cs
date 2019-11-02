using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReadBase : NetworkingFuncs
{
    protected NET_PACKET.NetworkDataManager NDM;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            NDM = FPSManager.FM.NDM;

            return true;
        }

        return false;
    }

    protected override void DestroyVars()
    {
        NDM = null;

        base.DestroyVars();
    }
}
