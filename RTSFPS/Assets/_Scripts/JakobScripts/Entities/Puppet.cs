using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Puppet : Entity
{
    JoinedVar<Puppet, Controller> instructor;
    public JoinedVar<Puppet, Controller> Instructor
    {
        get { return instructor; }
        protected set { instructor = value; }
    }

    JoinedList<Puppet, Permission> permissions;
    public JoinedList<Puppet, Permission> Permissions
    {
        get { return permissions; }
        protected set { permissions = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();

        AttachInstructor();
        WirePermissions();
    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();

        SetPermissions();
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        Instructor = new JoinedVar<Puppet, Controller>(this, false);
        Permissions = new JoinedList<Puppet, Permission>(this);
    }

    protected override void DeInitialize()
    {
        Instructor.Yeet();

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        Permissions.Yeet();

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        Permissions = null;
        Instructor = null;

        base.DestroyVars();
    }

    private void AttachInstructor()
    {
        if (Container.GetObj(0) != null)
        {
            ControllerSlot cs = Container.GetObj(0).GetComponent<ControllerSlot>();
            if (cs != null && cs.ObjectSlot.GetObj(0) != null)
            {
                Controller c = EType<Controller>.FindType(cs.ObjectSlot.GetObj(0));
                Instructor.Attach(c.Puppets);
                return;
            }
        }
        Instructor.Yeet();
    }

    private void SetPermissions()
    {
        Permission[] p = GetComponents<Permission>();
        for (int i = 0; i < p.Length; i++)
        {
            p[i].Init();
            Permissions.Attach(p[i].Actor);
        }
    }

    private void WirePermissions()
    {
        for (int i = Permissions.Joins.Count - 1; i >= 0; i--)
        {
            Permissions.GetObj(i).AttachCommands();
        }
    }

    protected override void OnBeforeTransformParentChanged()
    {
        base.OnBeforeTransformParentChanged();
    }

    //protected override void OnTransformParentChanged()
    //{
    //    base.OnTransformParentChanged();
    //
    //    AttachInstructor();
    //    for (int i = 0; i < Permissions.Joins.Count; i++)
    //    {
    //        Permissions.GetObj(i).YeetComm();
    //        Permissions.GetObj(i).AttachCommands();
    //    }
    //}

    protected override bool OnReparent()
    {
        if (base.OnReparent())
        {
            AttachInstructor();
            for (int i = 0; i < Permissions.Joins.Count; i++)
            {
                Permissions.GetObj(i).YeetComm();
                Permissions.GetObj(i).AttachCommands();
            }
            return true;
        }
        return false;
    }

    protected override void PostDisable()
    {
        if (!enabled)
        {
            for (int i = Permissions.Joins.Count - 1; i >= 0; --i)
                Permissions.GetObj(i).enabled = false;
        }

        base.PostDisable();
    }
}
