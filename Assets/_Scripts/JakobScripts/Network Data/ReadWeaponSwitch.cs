using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class ReadWeaponSwitch : ReadBase
{
    Inventory inv;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            inv = GetComponent<Inventory>();

            AddFirst();

            return true;
        }

        return false;
    }

    protected override void DestroyVars()
    {
        inv = null;

        base.DestroyVars();
    }

    protected override void First()
    {
        if (NDM != null && NDM.Init())
        {
            if (inv.contain)
            {
                //Debug.Log(NET_PACKET.NetworkDataManager.weaponStates[b.ID]);
                if (NET_PACKET.NetworkDataManager.weaponStates[inv.ID].flag)
                {
                    NET_PACKET.NetworkDataManager.weaponStates[inv.ID].flag = false;
                    inv.SwapActive((int)NET_PACKET.NetworkDataManager.weaponStates[inv.ID].WeaponState);
                    //Debug.Log(NET_PACKET.NetworkDataManager.weaponStates[b.ID].WeaponState);
                }
            }
        }
    }
}
