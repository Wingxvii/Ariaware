using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : Entity
{
    JoinedList<Controller, Command> commands;
    public JoinedList<Controller, Command> Commands
    {
        get { return commands; }
        protected set { commands = value; }
    }

    JoinedList<Controller, Puppet> puppets;
    public JoinedList<Controller, Puppet> Puppets
    {
        get { return puppets; }
        protected set { puppets = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();

        SetPuppets();
        WireCommands();
    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();

        SetCommands();
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        Puppets = new JoinedList<Controller, Puppet>(this);
        Commands = new JoinedList<Controller, Command>(this);
    }

    protected override void DeInitialize()
    {
        Puppets.Yeet();

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        Commands.Yeet();

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        Commands = null;
        Puppets = null;

        base.DestroyVars();
    }

    protected override SlotBase GetSlot()
    {
        return Container.GetObj(0).GetComponent<ControllerSlot>();
    }

    private void SetPuppets()
    {
        Puppets.Yeet();
        if (Container.GetObj(0) != null)
            for (int i = Container.GetObj(0).ObjectList.Joins.Count - 1; i >= 0; i--)
            {
                Puppet p = EType<Puppet>.FindType(Container.GetObj(0).ObjectList.GetObj(i));
                if (p != null)
                {
                    Puppets.Attach(p.Instructor);
                }
            }
    }

    private void SetCommands()
    {
        Command[] c = GetComponents<Command>();
        for (int i = 0; i < c.Length; i++)
        {
            c[i].Init();
            Commands.Attach(c[i].Instructor);
        }
    }

    private void WireCommands()
    {
        for (int i = Commands.Joins.Count - 1; i >= 0; i--)
        {
            Commands.GetObj(i).AttachPermissions();
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
    //    SetPuppets();
    //    for (int i = 0; i < Commands.Joins.Count; i++)
    //    {
    //        Commands.GetObj(i).YeetPerm();
    //        Commands.GetObj(i).AttachPermissions();
    //    }
    //}

    protected override bool OnReparent()
    {
        if (base.OnReparent())
        {
            SetPuppets();
            for (int i = 0; i < Commands.Joins.Count; i++)
            {
                Commands.GetObj(i).YeetPerm();
                Commands.GetObj(i).AttachPermissions();
            }
            return true;
        }
        return false;
    }

    protected override void PostDisable()
    {
        if (!enabled)
        {
            for (int i = Commands.Joins.Count - 1; i >= 0; --i)
                Commands.GetObj(i).enabled = false;
        }

        base.PostDisable();
    }
}
