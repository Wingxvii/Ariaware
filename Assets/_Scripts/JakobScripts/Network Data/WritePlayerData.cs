using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[RequireComponent(typeof(Puppet))]
public class WritePlayerData : WriteBase
{
    public Puppet p { get; protected set; }

    public Vector3 sendPos { get; set; }
    public Vector3 sendRot { get; set; }
    public uint sendState { get; set; }

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            p = GetComponent<Puppet>();

            AddFifth();

            return true;
        }

        return false;
    }

    protected override void Fifth()
    {
        //Debug.Log("HAPPENING");
        PlayerSourceEvent.FireEvent(new PlayerSourceEvent(this));

        StringBuilder sb = new StringBuilder();
        sb.Append(sendPos.x); sb.Append(",");
        sb.Append(sendPos.y); sb.Append(",");
        sb.Append(sendPos.z); sb.Append(",");
        sb.Append(sendRot.x); sb.Append(",");
        sb.Append(sendRot.y); sb.Append(",");
        sb.Append(sendRot.z); sb.Append(",");
        sb.Append(sendState); sb.Append(",");

        NET_PACKET.NetworkDataManager.SendNetData((int)NET_PACKET.PacketType.PLAYERDATA, sb.ToString());
    }
}
