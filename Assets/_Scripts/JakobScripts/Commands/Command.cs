using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller))]
public abstract class Command : UpdateableObject
{
    public JoinedVar<Command, Controller> Instructor;

    public JoinedList<Command, Permission> Receiver;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            Instructor = new JoinedVar<Command, Controller>(this);
            Receiver = new JoinedList<Command, Permission>(this);

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            if (!SetInstructor(GetComponent<Controller>()))
            {
                return false;
            }

            return true;
        }

        return false;
    }

    protected override bool CrossBranchInitialize()
    {
        if (base.CrossBranchInitialize())
        {
            AttachPermissions();

            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {
        Receiver.Yeet();

        base.CrossBranchDeInitialize();
    }

    protected override void InnerDeInitialize()
    {
        Instructor.Yeet();

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        Receiver = null;
        Instructor = null;

        base.DestroyVars();
    }

    public void AttachPermissions()
    {
        Controller c = Instructor.GetObj(0);
        if (c.TreeInit())
        {
            EntityContainer ec = c.Container.GetObj(0);
            if (ec != null && ec.TreeInit())
            {
                for (int i = 0; i < ec.AttachedSlots.Amount; ++i)
                {
                    SlotBase sb = ec.AttachedSlots.GetObj(i);
                    if (FType.FindIfType(sb.GetSlotType(), typeof(Puppet)) && sb.TreeInit())
                    {
                        for (int j = 0; j < sb.EntityPlug.Amount; ++j)
                        {
                            Puppet p = EType<Puppet>.FindType(sb.EntityPlug.GetObj(j));
                            if (p != null && p.TreeInit())
                            {
                                for (int k = 0; k < p.permissions.Amount; ++k)
                                {
                                    Permission pe = p.permissions.GetObj(k);
                                    SetPerm(pe);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public abstract void FeedPermissions();

    public abstract bool SetInstructor(Controller c);

    protected abstract bool SetPerm(Permission p);

    protected override bool PostEnable()
    {
        if (base.PostEnable())
        {
            if (!Instructor.GetObj(0).enabled)
            {
                enabled = false;
                return false;
            }

            return true;
        }

        return false;
    }
}
