using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPermission<T, U, V, W> : Permission where T : AbstractPermission<T, U, V, W> where U : AbstractCommand<U, T, W, V> where V : Puppet where W : Controller
{
    public V SpecificActor;

    public JoinedVar<T, U> SpecificEmitter;

    public ChannelTypes channel;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            SpecificEmitter = new JoinedVar<T, U>((T)this);

            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {
        SpecificEmitter.Yeet();

        base.CrossBranchDeInitialize();
    }

    protected override void InnerDeInitialize()
    {
        SpecificActor = null;

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        SpecificEmitter = null;

        base.DestroyVars();
    }

    public override bool SetActor(Puppet p)
    {
        V P = EType<V>.FindType(p);
        if (P != null && P.InnerInit())
        {
            SpecificActor = P;
            Actor.Attach(P.permissions);
            return true;
        }
        return false;
    }

    protected override bool SetComm(Command c)
    {
        U C = EType<U>.FindType(c);
        if (C != null && C.TreeInit())
        {
            SpecificEmitter.Attach(C.SpecificReceiver);
            Emitter.Attach(C.Receiver);
            return true;
        }
        return false;
    }

    private void FixedUpdate()
    {
        FeedPuppet();
    }
}
