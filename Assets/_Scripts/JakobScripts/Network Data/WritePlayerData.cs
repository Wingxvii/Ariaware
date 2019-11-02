using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

[RequireComponent(typeof(Puppet))]
public class WritePlayerData : WriteBase
{
    Puppet p;
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
        StringBuilder sb = new StringBuilder();
        sb.Append(transform.position.x); sb.Append(",");
        sb.Append(transform.position.y); sb.Append(",");
        sb.Append(transform.position.z); sb.Append(",");
        sb.Append(transform.position.x); sb.Append(",");
        sb.Append(transform.position.y); sb.Append(",");
        sb.Append(transform.position.z); sb.Append(",");
        sb.Append(transform.position.x); sb.Append(",");

        NET_PACKET.NetworkDataManager.SendNetData()
    }
}
