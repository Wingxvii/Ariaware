using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinedArray<T, U> : Joined<T, U> where T : InitializableObject where U : InitializableObject
{
    int arrSize = 1;
    public JoinedArray(T targ, int size)
    {
        Obj = targ;
        arrSize = Mathf.Max(size, 1);
    }

    Joined<U, T>[] joins;
    public Joined<U, T>[] Joins
    {
        get { if (joins == null) { joins = new JoinedArray<U, T>[arrSize]; } return joins; }
        protected set { joins = value; }
    }

    protected override bool SettingPartner(Joined<U, T> join, int slot = -1)
    {
        if (slot >= 0 && slot < Joins.Length)
        {
            if (Joins[slot] != null)
            {
                Joins[slot].Remove(this, true);
                //Remove(Joins[slot], true);
            }
            Joins[slot] = join;

            return true;
        }

        return false;
    }

    public override void Yeet(bool kick = false)
    {
        for (int i = Joins.Length - 1; i >= 0; --i)
        {
            if (Joins[i] != null)
            {
                Joins[i].Remove(this, kick);
                Joins[i] = null;
            }
        }
    }

    protected override void Removing(Joined<U, T> join, bool kick = false)
    {
        for (int i = Joins.Length - 1; i >= 0; --i)
        {
            if (Joins[i] == join)
            {
                Joins[i] = null;
            }
        }
    }

    public override Joined<U, T> GetPartner(int i)
    {
        if (i >= 0 && i < Joins.Length)
            return Joins[i];
        return null;
    }

    public override U GetObj(int i)
    {
        if (i >= 0 && i < Joins.Length)
            if (Joins[i] != null)
                return Joins[i].Obj;
        return null;
    }

    public override bool checkInBounds(int index)
    {
        if (index < 0)
            return true;
        return (index >= 0 && index < Joins.Length);
    }

    public override int Contains(Joined<U, T> j)
    {
        return j.SelfContains(this);
    }

    public override int SelfContains(Joined<U, T> j)
    {
        for (int i = Joins.Length - 1; i >= 0; --i)
            if (Joins[i] == j)
                return i;
        return -1;
    }

    protected override int JoinSize()
    {
        return Joins.Length;
    }
}
