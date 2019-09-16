using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCommand<T, U, V, W> : Command where T : AbstractCommand<T, U, V, W> where U : AbstractPermission<U, T, W, V> where V : Controller where W : Puppet
{
    V specificInstructor;
    public V SpecificInstructor
    {
        get { return specificInstructor; }
        protected set { specificInstructor = value; }
    }

    JoinedList<T, U> specificReceiver;

    public JoinedList<T, U> SpecificReceiver
    {
        get { return specificReceiver; }
        protected set { specificReceiver = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();

        
    }

    protected override bool SetPerm(Permission p)
    {
        U P = EType<U>.FindType(p);
        if (P != null)
        {
            P.Init();
            P.InnerInit();
            SpecificReceiver.Attach(P.SpecificEmitter);
            Receiver.Attach(P.Emitter);
            return true;
        }
        return false;
    }

    protected override void YeetSpecificPerm()
    {
        SpecificReceiver.Yeet();
    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();

        SpecificInstructor = EType<V>.FindType(Instructor.GetObj(0));
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        SpecificReceiver = new JoinedList<T, U>((T)this);
    }

    protected override void DeInitialize()
    {


        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        SpecificReceiver.Yeet();

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        SpecificReceiver = null;

        base.DestroyVars();
    }

    private void Update()
    {
        for (int i = SpecificReceiver.Joins.Count - 1; i >= 0; i--)
            FeedPermission(SpecificReceiver.GetObj(i));
    }

    protected abstract void FeedPermission(U p);
}
