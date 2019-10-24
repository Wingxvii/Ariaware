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

    protected override bool SettingPartner(Joined<U, T> join, int slot = -1)
    {
        if (slot >= 0 && slot < Joins.Count)
        {
            Joins[slot].Remove(this, true);
            Remove(join, true);
            Joins.Insert(slot, join);
        }
        else
            Joins.Add(join);

        return true;
    }

    public override void Yeet(bool kick = false)
    {
        for (int i = 0; i < Joins.Count; i++)
            Joins[i].Remove(this, kick);
        Joins.Clear();
    }

    protected override void Removing(Joined<U, T> join, bool kick = false)
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

    public override int Contains(Joined<U, T> j)
    {
        return j.SelfContains(this);
    }

    public override int SelfContains(Joined<U, T> j)
    {
        for (int i = Joins.Count - 1; i >= 0; --i)
            if (Joins[i] == j)
                return i;
        return -1;
    }

    protected override int JoinSize()
    {
        return Joins.Count;
    }
}
