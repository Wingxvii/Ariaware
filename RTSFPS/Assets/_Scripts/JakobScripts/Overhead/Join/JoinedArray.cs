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

    public override void SetPartner(Joined<U, T> join, int slot = -1)
    {
        if (slot >= 0 && slot < Joins.Length)
        {
            Joins[slot].Remove(this);
            Remove(Joins[slot]);
            Joins[slot] = join;
        }
    }

    public override void Yeet(bool EnableKickFromParent = false)
    {
        for (int i = Joins.Length - 1; i >= 0; --i)
        {
            if (Joins[i] != null)
            {
                Joins[i].Remove(this);
                Joins[i] = null;
            }
        }
    }

    public override void Remove(Joined<U, T> join, bool EnableKickFromParent = false)
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
}
