using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : BasePACES
{
    public JoinedVar<Entity, EntityContainer> Container;
    public JoinedVar<Entity, SlotBase> AttachedSlot;

    public JoinedList<Entity, Modifier> Modifiers;
    public JoinedList<Entity, ObjectStat> JoinedStats;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            Container = new JoinedVar<Entity, EntityContainer>(this, true);
            AttachedSlot = new JoinedVar<Entity, SlotBase>(this, true);

            Modifiers = new JoinedList<Entity, Modifier>(this);
            JoinedStats = new JoinedList<Entity, ObjectStat>(this);

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            AttachObjectStats();
            AttachModifiers();
            
            return true;
        }

        return false;
    }

    protected override bool HierarchyInitialize()
    {
        if (base.HierarchyInitialize())
        {
            if (!AttachContainer())
                return false;

            if (!AttachSlot())
                return false;

            return true;
        }

        return false;
    }

    protected override void HierarchyDeInitialize()
    {
        AttachedSlot.Yeet();
        Container.Yeet();

        base.HierarchyDeInitialize();
    }

    protected override void InnerDeInitialize()
    {
        Modifiers.Yeet();
        JoinedStats.Yeet();

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        AttachedSlot = null;
        Container = null;

        base.DestroyVars();
    }

    protected void OnBeforeTransformParentChanged()
    {
        OnPreReparent();
    }

    protected virtual void OnPreReparent()
    {
        //BranchDeInit();
        gameObject.SetActive(false);
    }

    protected void OnTransformParentChanged()
    {
        OnReparent();
    }

    protected virtual void OnReparent()
    {
        //TreeInit();
        gameObject.SetActive(true);
    }

    public void AttachObjectStats()
    {
        ObjectStat[] os = GetComponents<ObjectStat>();
        for (int i = 0; i < os.Length; ++i)
        {
            if (os[i].InnerInit())
            {
                JoinedStats.Attach(os[i].Ent);
            }
        }
    }

    public void AttachModifiers()
    {
        Modifier[] m = GetComponents<Modifier>();
        for (int i = 0; i < m.Length; ++i)
        {
            if (m[i].InnerInit())
            {
                Modifiers.Attach(m[i].BaseEntity);
            }
        }
    }

    public bool AttachContainer()
    {
        EntityContainer[] ec = GetComponentsInParent<EntityContainer>();

        if (ec.Length > 0)
        {
            for (int i = 0; i < ec.Length; ++i)
            {
                if (ec[i].BranchInit())
                {
                    Container.Attach(ec[i].AttachedEntities);
                    ID = ec[i].ID;
                    return true;
                }
            }

            Container.Yeet(true);
        }

        return false;
    }

    public bool AttachSlot()
    {
        for (int i = Container.GetObj(0).AttachedSlots.Amount - 1; i >= 0; --i)
        {
            SlotBase sb = Container.GetObj(0).AttachedSlots.GetObj(i);
            if (sb.BranchInit() && sb.CullEntity(this))
            {
                AttachedSlot.Attach(sb.EntityPlug);
                return true;
            }
        }

        AttachedSlot.Yeet(true);

        return false;
    }
}
