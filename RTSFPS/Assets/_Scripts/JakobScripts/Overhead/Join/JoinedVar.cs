using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinedVar<T, U> : Joined<T, U> where T : InitializableObject where U : InitializableObject
{
    public JoinedVar(T targ, bool kickWhenRemoved)
    {
        Obj = targ;
        KickFromParentWhenRemoved = kickWhenRemoved;
    }

    bool kickFromParentWhenRemoved = false;
    public bool KickFromParentWhenRemoved
    {
        get { return kickFromParentWhenRemoved; }
        protected set { kickFromParentWhenRemoved = value; }
    }

    public Joined<U, T> Joins { get; protected set; }

    public override void SetPartner(Joined<U, T> join, int slot = -1)
    {
        if (Joins != join)
        {
            if (Joins != null)
                Joins.Remove(this, true);
            Joins = join;
        }
    }

    public override void Yeet(bool EnableKickFromParent = false)
    {
        if (Joins != null)
        {
            Joins.Remove(this, true);
        }
        Remove(Joins, EnableKickFromParent);
    }

    public override void Remove(Joined<U, T> join, bool EnableKickFromParent = false)
    {
        //Debug.Log(Obj.name + " ---> Aaaand");
        if (Joins == join)// && Joins != null)
        {
            Joins = null;
            //Debug.Log(Obj.name + " ---> AAAAAAAND! " + (KickFromParentWhenRemoved && EnableKickFromParent));
            if (Obj.transform.parent != null && KickFromParentWhenRemoved && EnableKickFromParent && Obj.isActiveAndEnabled)
            {
                //Debug.Log(Obj.name + " ---> THERE!");
                Obj.transform.SetParent(null);
            }
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
}
