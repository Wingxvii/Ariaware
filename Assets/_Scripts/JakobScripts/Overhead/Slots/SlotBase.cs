using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityContainer))]
public abstract class SlotBase : UpdateableObject
{
    public JoinedVar<SlotBase, EntityContainer> Container;
    public Joined<SlotBase, Entity> EntityPlug;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            Container = new JoinedVar<SlotBase, EntityContainer>(this);

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            EntityContainer ec = GetComponent<EntityContainer>();
            if (ec.InnerInit())
            {
                Container.Attach(ec.AttachedSlots);
            }

            return true;
        }

        return false; 
    }

    protected override void HierarchyDeInitialize()
    {
        EntityPlug.Yeet(AC);

        base.HierarchyDeInitialize();
    }

    protected override void InnerDeInitialize()
    {
        Container.Yeet();

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        EntityPlug = null;
        Container = null;

        base.DestroyVars();
    }

    protected override bool PostEnable()
    {
        if (base.PostEnable())
        {
            if (!Container.GetObj(0).enabled)
            {
                enabled = false;
                return false;
            }

            return true;
        }

        return false;
    }

    public abstract bool CullEntity(Entity e);

    public abstract System.Type GetSlotType();
}
