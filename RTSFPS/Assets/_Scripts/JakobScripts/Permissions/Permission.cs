using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Puppet))]
public abstract class Permission : InitializableObject
{
    JoinedVar<Permission, Puppet> actor;
    public JoinedVar<Permission, Puppet> Actor
    {
        get { return actor; }
        protected set { actor = value; }
    }

    JoinedVar<Permission, Command> emitter;
    public JoinedVar<Permission, Command> Emitter
    {
        get { return emitter; }
        protected set { emitter = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();

        AttachCommands();
    }

    public void AttachCommands()
    {
        Controller c = Actor.GetObj(0).Instructor.GetObj(0);
        if (c != null)
        {
            for (int i = c.Commands.Joins.Count - 1; i >= 0; i--)
            {
                Command comm = c.Commands.GetObj(i);
                if (SetComm(comm))
                    i = -1;
            }
        }
    }

    protected abstract bool SetComm(Command c);

    public void YeetComm()
    {
        Emitter.Yeet();
        YeetSpecificComm();
    }

    protected abstract void YeetSpecificComm();

    protected override void InnerInitialize()
    {
        base.InnerInitialize();

        Puppet p = GetComponent<Puppet>();
        p.Init();
        Actor.Attach(p.Permissions);
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        Actor = new JoinedVar<Permission, Puppet>(this, false);
        Emitter = new JoinedVar<Permission, Command>(this, false);
    }

    protected override void DeInitialize()
    {
        Emitter.Yeet();

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        Actor.Yeet();

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        Emitter = null;
        Actor = null;

        base.DestroyVars();
    }

    protected abstract void FeedPuppet();

    protected override void PostEnable()
    {
        base.PostEnable();

        if (Actor.GetObj(0) != null)
            Actor.GetObj(0).enabled = true;
    }
}
