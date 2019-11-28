using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[RequireComponent(typeof(Inventory))]
public class WriteWeaponSwitch : WriteBase
{
    Inventory i;
    int lastState = 0;
    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            i = GetComponent<Inventory>();

            return true;
        }

        return false;
    }

    protected override void Second()
    {
        if (i.activeObject != lastState)
        {
            lastState = i.activeObject;
            StringBuilder sb = new StringBuilder();

            sb.Append(lastState); sb.Append(",");

            NET_PACKET.NetworkDataManager.SendNetData((int)NET_PACKET.PacketType.WEAPONSTATE, sb.ToString());
        }
    }
}
