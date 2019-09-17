using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller))]
public abstract class Command : InitializableObject
{
    JoinedVar<Command, Controller> instructor;
    public JoinedVar<Command, Controller> Instructor
    {
        get { return instructor; }
        protected set { instructor = value; }
    }

    JoinedList<Command, Permission> receiver;
    public JoinedList<Command, Permission> Receiver
    {
        get { return receiver; }
        protected set { receiver = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();

        AttachPermissions();
    }

    public void AttachPermissions()
    {
        YeetPerm();
        for (int i = Instructor.GetObj(0).Puppets.Joins.Count - 1; i >= 0; i--)
        {
            Puppet p = Instructor.GetObj(0).Puppets.GetObj(i);
            for (int j = p.Permissions.Joins.Count - 1; j >= 0; j--)
            {
                Permission perm = p.Permissions.GetObj(j);
                SetPerm(perm);
            }
        }
    }

    protected abstract bool SetPerm(Permission p);

    public void YeetPerm()
    {
        Receiver.Yeet();
        YeetSpecificPerm();
    }

    protected abstract void YeetSpecificPerm();

    protected override void InnerInitialize()
    {
        base.InnerInitialize();

        Controller c = GetComponent<Controller>();
        c.Init();
        Instructor.Attach(c.Commands);
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        Instructor = new JoinedVar<Command, Controller>(this, false);
        Receiver = new JoinedList<Command, Permission>(this);
    }

    protected override void DeInitialize()
    {
        Receiver.Yeet();

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        Instructor.Yeet();

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        Instructor = null;
        Receiver = null;

        base.DestroyVars();
    }

    protected override void PostEnable()
    {
        base.PostEnable();

        if (Instructor.GetObj(0) != null)
            Instructor.GetObj(0).enabled = true;
    }
}
