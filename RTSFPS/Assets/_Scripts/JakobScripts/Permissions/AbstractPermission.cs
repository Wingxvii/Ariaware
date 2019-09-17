using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPermission<T, U, V, W> : Permission where T : AbstractPermission<T, U, V, W> where U : AbstractCommand<U, T, W, V> where V : Puppet where W : Controller
{
    V specificActor;
    public V SpecificActor
    {
        get { return specificActor; }
        protected set { specificActor = value; }
    }

    JoinedVar<T, U> specificEmitter;
    public JoinedVar<T, U> SpecificEmitter
    {
        get { return specificEmitter; }
        protected set { specificEmitter = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();

        
    }

    protected override bool SetComm(Command c)
    {
        U C = EType<U>.FindType(c);
        if (C != null)
        {
            C.Init();
            C.InnerInit();
            SpecificEmitter.Attach(C.SpecificReceiver);
            Emitter.Attach(C.Receiver);
            return true;
        }
        return false;
    }

    protected override void YeetSpecificComm()
    {
        SpecificEmitter.Yeet();
    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();

        SpecificActor = EType<V>.FindType(Actor.GetObj(0));
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        SpecificEmitter = new JoinedVar<T, U>((T)this, false);
    }

    protected override void DeInitialize()
    {
        SpecificEmitter.Yeet();

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {


        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        SpecificEmitter = null;

        base.DestroyVars();
    }

    private void FixedUpdate()
    {
        FeedPuppet();
    }
}
