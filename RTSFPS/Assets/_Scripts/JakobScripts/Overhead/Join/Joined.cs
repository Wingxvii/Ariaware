﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Joined <T, U> where T : InitializableObject where U : InitializableObject
{
    T obj;
    public T Obj
    {
        get { return obj; }
        protected set { obj = value; }
    }

    public int Amount
    {
        get { return JoinSize(); }
    }

    public void Attach(Joined<U, T> join, int otherSlot = -1, int thisSlot = -1)
    {
        if (join != null && Contains(join) < 0)
        {
            SetPartner(join, thisSlot);
            join.SetPartner(this, otherSlot);
        }
    }

    public void AttachWithSlotsReversed(Joined<U, T> join, int thisSlot = -1, int otherSlot = -1)
    {
        Attach(join, otherSlot, thisSlot);
    }

    public abstract void SetPartner(Joined<U, T> join, int slot = -1);

    public abstract void Yeet(bool kick = false);

    public abstract void Remove(Joined<U, T> join, bool kick = false);

    public abstract Joined<U, T> GetPartner(int i);

    public abstract U GetObj(int i);

    public abstract bool checkInBounds(int index);

    public abstract int Contains(Joined<U, T> j);

    public abstract int SelfContains(Joined<U, T> j);

    protected abstract int JoinSize();
}