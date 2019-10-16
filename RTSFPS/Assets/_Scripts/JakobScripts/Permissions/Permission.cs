using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Puppet))]
public abstract class Permission : InitializableObject
{
    public JoinedVar<Permission, Puppet> Actor;

    public JoinedVar<Permission, Command> Emitter;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            Actor = new JoinedVar<Permission, Puppet>(this);
            Emitter = new JoinedVar<Permission, Command>(this);

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            if (!SetActor(GetComponent<Puppet>()))
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
            AttachCommands();

            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {
        Emitter.Yeet();
        base.CrossBranchDeInitialize();
    }

    protected override void InnerDeInitialize()
    {
        Actor.Yeet();
        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        Emitter = null;
        Actor = null;

        base.DestroyVars();
    }

    public void AttachCommands()
    {
        Puppet p = Actor.GetObj(0);
        if (p.TreeInit())
        {
            EntityContainer ec = Actor.GetObj(0).Container.GetObj(0);
            if (ec != null && ec.TreeInit())
            {
                for (int i = 0; i < ec.AttachedSlots.Amount; ++i)
                {
                    SlotBase sb = ec.AttachedSlots.GetObj(i);
                    if (sb.TreeInit() && FType.FindIfType(sb.GetSlotType(), typeof(Controller)))
                    {
                        for (int j = 0; j < sb.EntityPlug.Amount; ++j)
                        {
                            Controller c = EType<Controller>.FindType(sb.EntityPlug.GetObj(j));
                            if (c != null && c.TreeInit())
                            {
                                for (int k = 0; k < c.commands.Amount; ++k)
                                {
                                    Command co = c.commands.GetObj(k);
                                    if (SetComm(co))
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }
    }

    protected abstract bool SetComm(Command c);

    public abstract bool SetActor(Puppet p);

    protected abstract void FeedPuppet();

    protected override bool PostEnable()
    {
        if (base.PostEnable())
        {
            if (!Actor.GetObj(0).enabled)
            {
                enabled = false;
                return false;
            }

            return true;
        }

        return false;
    }
}
