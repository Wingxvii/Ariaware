using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityContainer : BasePACES
{
    public JoinedList<EntityContainer, Entity> AttachedEntities;
    public JoinedList<EntityContainer, SlotBase> AttachedSlots;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AttachedSlots = new JoinedList<EntityContainer, SlotBase>(this);
            AttachedEntities = new JoinedList<EntityContainer, Entity>(this);

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            AttachSlots();

            return true;
        }

        return false;
    }

    protected override bool HierarchyInitialize()
    {
        if (base.HierarchyInitialize())
        {
            AttachEntities();

            return true;
        }

        return false;
    }

    protected override void HierarchyDeInitialize()
    {
        AttachedEntities.Yeet(AC);

        base.HierarchyDeInitialize();
    }

    protected override void InnerDeInitialize()
    {
        AttachedSlots.Yeet();

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        AttachedEntities = null;
        AttachedSlots = null;

        base.DestroyVars();
    }

    public void AttachSlots()
    {
        SlotBase[] s = GetComponents<SlotBase>();
        for (int i = s.Length - 1; i >= 0; --i)
        {
            if (s[i].InnerInit())
                AttachedSlots.Attach(s[i].Container);
        }
    }

    public void AttachEntities()
    {
        Entity[] e = GetComponentsInChildren<Entity>();
        for (int i = e.Length - 1; i >= 0; --i)
        {
            if (e[i].BranchInit())
                AttachedEntities.Attach(e[i].Container);
        }
    }

    protected override bool PostEnable()
    {
        if (base.PostEnable())
        {
            for (int i = 0; i < AttachedSlots.Amount; i++)
            {
                AttachedSlots.GetObj(i).AutoEnable();
            }

            return true;
        }

        return false;
    }

    protected override void PostDisable()
    {
        if (AC)
        {
            for (int i = 0; i < AttachedSlots.Amount; i++)
            {
                AttachedSlots.GetObj(i).AutoDisable();
            }
        }

        base.PostDisable();
    }

    public enum ContainerType
    {
        None,
        Player,
        Enemy,
        Turret,
        Barrack,
        Wall
    }
}
