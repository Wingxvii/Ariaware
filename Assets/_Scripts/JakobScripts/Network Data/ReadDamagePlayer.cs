using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayerUnit
{
    public DamagePlayerUnit(int _dmg, uint _culprit)
    {
        dmg = _dmg;
        culprit = _culprit;
    }

    public int dmg;
    public uint culprit;
}

[RequireComponent(typeof(Body))]
public class ReadDamagePlayer : ReadBase
{
    public Body b { get; protected set; }

    public Queue<DamagePlayerUnit> Damages { get; protected set; }

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddThird();

            if (FPSManager.FM.NDM.Init())
                NET_PACKET.NetworkDataManager.damagesPlayer.Add(this);

            Damages = new Queue<DamagePlayerUnit>();

            b = GetComponent<Body>();

            return true;
        }

        return false;
    }

    protected override void DestroyVars()
    {
        b = null;

        base.DestroyVars();
    }

    protected override void Third()
    {
        if (NDM != null && NDM.Init())
        {
            NDM.ChugDamagePlayerQueue();
        }

        while (Damages.Count > 0)
        {
            Debug.Log("AHA");
            b.Damage(Damages.Peek().dmg);
            Damages.Dequeue();
        }
    }
}
