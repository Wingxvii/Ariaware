using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : Entity
{
    public JoinedList<Controller, Puppet> puppets;
    public JoinedList<Controller, Command> commands;

    public Slider HPbar;
    public Text ammo;
    public Text clip;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddUpdate();
            
            puppets = new JoinedList<Controller, Puppet>(this);
            commands = new JoinedList<Controller, Command>(this);

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            AttachCommands();

            return true;
        }

        return false;
    }

    protected override bool CrossBranchInitialize()
    {
        if (base.CrossBranchInitialize())
        {
            AttachPuppets();

            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {
        puppets.Yeet();

        base.CrossBranchDeInitialize();
    }

    protected override void InnerDeInitialize()
    {
        commands.Yeet();

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        commands = null;
        puppets = null;

        base.DestroyVars();
    }

    void AttachPuppets()
    {
        EntityContainer ec = Container.GetObj(0);
        if (ec.TreeInit())
        {
            for (int i = 0; i < ec.AttachedSlots.Amount; ++i)
            {
                SlotBase sb = ec.AttachedSlots.GetObj(i);
                if (sb.BranchInit() && FType.FindIfType(sb.GetSlotType(), typeof(Puppet)))
                {
                    for (int j = 0; j < sb.EntityPlug.Amount; ++j)
                    {
                        Puppet p = EType<Puppet>.FindType(sb.EntityPlug.GetObj(j));
                        if (p != null && p.TreeInit())
                        {
                            puppets.Attach(p.controller);
                            HealthBar hb = p.GetComponent<HealthBar>();
                            if (hb != null)
                            {
                                hb.hpBar = HPbar;
                            }
                            PACES.Gun pGun = p.GetComponent<PACES.Gun>();
                            if (pGun != null)
                            {
                                pGun.ammoCount = ammo;
                                pGun.ammoClip = clip;
                            }
                        }
                    }
                }
            }
        }
    }

    void AttachCommands()
    {
        Command[] c = GetComponents<Command>();
        for (int i = c.Length - 1; i >= 0; --i)
        {
            if (c[i].InnerInit())
            {
                if (!c[i].SetInstructor(this))
                {

                }
            }
        }
    }

    protected override void UpdateObject()
    {
        for (int i = 0; i < commands.Amount; ++i)
        {
            commands.GetObj(i).FeedPermissions();
        }
    }

    protected override bool PostEnable()
    {
        if (base.PostEnable())
        {
            for (int i = 0; i < commands.Amount; i++)
            {
                commands.GetObj(i).AutoEnable();
            }

            return true;
        }

        return false;
    }

    protected override void PostDisable()
    {
        if (AC)
        {
            for (int i = 0; i < commands.Amount; i++)
            {
                commands.GetObj(i).AutoDisable();
            }
        }

        base.PostDisable();
    }
}
