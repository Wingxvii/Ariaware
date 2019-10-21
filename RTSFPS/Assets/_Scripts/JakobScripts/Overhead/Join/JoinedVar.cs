using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Set this back to removing transforms
public class JoinedVar<T, U> : Joined<T, U> where T : InitializableObject where U : InitializableObject
{
    public JoinedVar(T targ, bool kick = false)
    {
        Obj = targ;
        KickOnRemove = kick;
    }

    public bool KickOnRemove { get; private set; }

    public Joined<U, T> Joins { get; protected set; }

    protected override bool SettingPartner(Joined<U, T> join, int slot = -1)
    {
        if (Joins != join)
        {
            if (Joins != null)
                Joins.Remove(this, true);
            Joins = join;

            return true;
        }

        return false;
    }

    public override void Yeet(bool kick = false)
    {
        if (Joins != null)
        {
            Joins.Remove(this, kick);
        }
        Remove(Joins, kick);
    }

    protected override void Removing(Joined<U, T> join, bool kick = false)
    {
        if (Joins == join)
        {
            Joins = null;

            if (kick && KickOnRemove)
                Obj.transform.SetParent(null);
        }
    }

    public override Joined<U, T> GetPartner(int i)
    {
        return Joins;
    }

    public override U GetObj(int i)
    {
        if (Joins == null)
            return null;
        return Joins.Obj;
    }

    public override bool checkInBounds(int index)
    {
        return true;
    }

    public override int Contains(Joined<U, T> j)
    {
        return SelfContains(j);
    }

    public override int SelfContains(Joined<U, T> j)
    {
        return Joins == j ? 0 : -1;
    }

    protected override int JoinSize()
    {
        return 1;
    }
}
