using System.Collections;
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

    public delegate void AttachFuncs(Joined<U, T> join);
    public delegate void RemoveFuncs(Joined<U, T> join);

    List<AttachFuncs> runOnAttach;
    public List<AttachFuncs> RunOnAttach
    {
        get { if (runOnAttach == null) { runOnAttach = new List<AttachFuncs>(); } return runOnAttach; }
        private set { runOnAttach = value; }
    }

    List<RemoveFuncs> runOnRemove;
    public List<RemoveFuncs> RunOnRemove
    {
        get { if (runOnRemove == null) { runOnRemove = new List<RemoveFuncs>(); } return runOnRemove; }
        private set { runOnRemove = value; }
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

    public void SetPartner(Joined<U, T> join, int slot = -1)
    {
        if (SettingPartner(join, slot))
        {
            for (int i = 0; i < RunOnAttach.Count; ++i)
            {
                RunOnAttach[i](join);
            }
        }
    }

    protected abstract bool SettingPartner(Joined<U, T> join, int slot = -1);

    public abstract void Yeet(bool kick = false);

    public void Remove(Joined<U, T> join, bool kick = false)
    {
        for (int i = 0; i < RunOnRemove.Count; ++i)
        {
            RunOnRemove[i](join);
        }

        Removing(join, kick);
    }

    protected abstract void Removing(Joined<U, T> join, bool kick = false);

    public abstract Joined<U, T> GetPartner(int i);

    public abstract U GetObj(int i);

    public abstract bool checkInBounds(int index);

    public abstract int Contains(Joined<U, T> j);

    public abstract int SelfContains(Joined<U, T> j);

    protected abstract int JoinSize();

    public int GetPositionOf(Joined<U, T> j)
    {
        return SelfContains(j);
    }
}