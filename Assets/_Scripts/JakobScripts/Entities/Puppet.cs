using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puppet : Entity
{
    public JoinedVar<Puppet, Controller> controller;
    public JoinedList<Puppet, Permission> permissions;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            controller = new JoinedVar<Puppet, Controller>(this);
            permissions = new JoinedList<Puppet, Permission>(this);

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            AttachPermissions();

            return true;
        }

        return false;
    }

    protected override bool CrossBranchInitialize()
    {
        if (base.CrossBranchInitialize())
        {
            AttachController();

            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {
        controller.Yeet();

        base.CrossBranchDeInitialize();
    }

    protected override void InnerDeInitialize()
    {
        permissions.Yeet();

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        controller = null;

        base.DestroyVars();
    }

    void AttachController()
    {
        EntityContainer ec = Container.GetObj(0);
        if (ec.TreeInit())
        {
            for (int i = 0; i < ec.AttachedSlots.Amount; ++i)
            {
                SlotBase sb = ec.AttachedSlots.GetObj(i);
                if (sb.BranchInit() && FType.FindIfType(sb.GetSlotType(), typeof(Controller)))
                {
                    for (int j = 0; j < sb.EntityPlug.Amount; ++j)
                    {
                        Controller c = EType<Controller>.FindType(sb.EntityPlug.GetObj(j));
                        if (c != null && c.TreeInit())
                        {
                            controller.Attach(c.puppets);
                            return;
                        }
                    }
                    return;
                }
            }
        }
    }

    void AttachPermissions()
    {
        Permission[] p = GetComponents<Permission>();
        for (int i = p.Length - 1; i >= 0; --i)
        {
            if (p[i].InnerInit())
            {
                if (!p[i].SetActor(this))
                {

                }
            }
        }
    }

    protected override bool PostEnable()
    {
        if (base.PostEnable())
        {
            for (int i = 0; i < permissions.Amount; i++)
            {
                permissions.GetObj(i).AutoEnable();
            }

            return true;
        }

        return false;
    }

    protected override void PostDisable()
    {
        if (AC)
        {
            for (int i = 0; i < permissions.Amount; i++)
            {
                permissions.GetObj(i).AutoDisable();
            }
        }

        base.PostDisable();
    }
}
