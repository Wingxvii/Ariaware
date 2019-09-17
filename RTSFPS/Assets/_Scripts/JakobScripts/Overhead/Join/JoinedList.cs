using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinedList<T, U> : Joined<T, U> where T : InitializableObject where U : InitializableObject
{
    public JoinedList(T targ)
    {
        Obj = targ;
    }

    List<Joined<U, T>> joins;
    public List<Joined<U, T>> Joins
    {
        get { if (joins == null) { joins = new List<Joined<U, T>>(); } return joins; }
        protected set { joins = value; }
    }

    public override void SetPartner(Joined<U, T> join, int slot = -1)
    {
        if (!Joins.Contains(join))
        {
            if (slot >= 0 && slot < Joins.Count)
            {
                Joins[slot].Remove(this);
                Remove(join);
                Joins.Insert(slot, join);
            }
            else
                Joins.Add(join);
        }
    }

    public override void Yeet(bool EnableKickFromParent = false)
    {
        for (int i = 0; i < Joins.Count; i++)
            Joins[i].Remove(this, true);
        Joins.Clear();
    }

    public override void Remove(Joined<U, T> join, bool EnableKickFromParent = false)
    {
        Joins.Remove(join);
    }

    public override Joined<U, T> GetPartner(int i)
    {
        if (i < Joins.Count && i >= 0)
            return Joins[i];
        return null;
    }

    public override U GetObj(int i)
    {
        if (i < Joins.Count && i >= 0)
            return Joins[i].Obj;
        return null;
    }

    public override bool checkInBounds(int index)
    {
        if (index < 0)
            return true;
        return (index >= 0 && index < Joins.Count);
    }
}
