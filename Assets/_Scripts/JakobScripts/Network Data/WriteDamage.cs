using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class HitIDUnit
{
    public HitIDUnit(int _client, int _id, int _damage)
    {
        client = _client;
        id = _id;
        damage = _damage;
    }

    public int id, damage, client;
}

public class WriteDamage : WriteBase
{
    public Queue<HitIDUnit> damages { get; protected set; }

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddFourth();

            damages = new Queue<HitIDUnit>();

            return true;
        }

        return false;
    }

    protected override void Fourth()
    {
        while (damages.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(damages.Peek().client); sb.Append(",");
            sb.Append(damages.Peek().damage); sb.Append(",");
            sb.Append(damages.Peek().id); sb.Append(",");

            NET_PACKET.NetworkDataManager.SendNetData((int)PacketType.DAMAGEDEALT, sb.ToString());
        }
    }
}
