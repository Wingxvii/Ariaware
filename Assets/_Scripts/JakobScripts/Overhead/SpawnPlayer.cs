using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NET_PACKET.NetworkDataManager))]
public class SpawnPlayer : InitializableObject
{
    public EntityContainer ControllablePlayer;
    public EntityContainer OtherPlayer;

    NET_PACKET.NetworkDataManager NDM;

    // Start is called before the first frame update
    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            NDM = GetComponent<NET_PACKET.NetworkDataManager>();

            SpawnLoc[] locs = GetComponentsInChildren<SpawnLoc>();
            for (int i = 0; i < locs.Length; ++i)
            {
                locs[i].CreateMe();
            }

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            if (NDM.Init())
            {
                for (int i = 0; i < NET_PACKET.NetworkDataManager.FPSmax; ++i)
                {
                    EntityContainer EC = null;
                    //Debug.Log(NDM.GetPlayerNum());
                    if (i == NDM.GetPlayerNum())
                    {
                        EC = Instantiate(ControllablePlayer, SpawnLoc.Locations[i % NET_PACKET.NetworkDataManager.FPSmax].transform.position, Quaternion.identity);
                    }
                    else
                    {
                        EC = Instantiate(OtherPlayer, SpawnLoc.Locations[i % NET_PACKET.NetworkDataManager.FPSmax].transform.position, Quaternion.identity);
                    }

                    EC.SetContainerID(i);
                }
            }

            return true;
        }

        return false;
    }

    protected override void InnerDeInitialize()
    {


        base.InnerDeInitialize();
    }


    protected override void DestroyVars()
    {
        NDM = null;

        base.DestroyVars();
    }


    public void ConstructNewPlayer(int index)
    {

    }
}
