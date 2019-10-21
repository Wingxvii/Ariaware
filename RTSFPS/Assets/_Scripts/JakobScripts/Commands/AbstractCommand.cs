using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCommand<T, U, V, W> : Command where T : AbstractCommand<T, U, V, W> where U : AbstractPermission<U, T, W, V> where V : Controller where W : Puppet
{
    public V SpecificInstructor;

    public JoinedList<T, U> SpecificReceiver;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            SpecificReceiver = new JoinedList<T, U>((T)this);

            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {
        SpecificReceiver.Yeet();

        base.CrossBranchDeInitialize();
    }

    protected override void InnerDeInitialize()
    {
        SpecificInstructor = null;

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        SpecificReceiver = null;

        base.DestroyVars();
    }

    public override bool SetInstructor(Controller c)
    {
        V C = EType<V>.FindType(c);
        if (C != null && C.InnerInit())
        {
            SpecificInstructor = C;
            Instructor.Attach(C.commands);
            return true;
        }
        return false;
    }

    protected override bool SetPerm(Permission p)
    {
        U P = EType<U>.FindType(p);
        if (P != null && P.TreeInit())
        {
            SpecificReceiver.Attach(P.SpecificEmitter);
            Receiver.Attach(P.Emitter);
            return true;
        }
        return false;
    }

    public override void FeedPermissions()
    {
        for (int i = 0; i < Receiver.Amount; ++i)
        {
            U sr = SpecificReceiver.GetObj(i);
            FeedPermission(sr);
        }
    }

    protected abstract void FeedPermission(U p);
}
