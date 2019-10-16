using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public abstract class Modifier : InitializableObject
{
    public JoinedVar<Modifier, Entity> BaseEntity;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            BaseEntity = new JoinedVar<Modifier, Entity>(this, false);

            return true;
        }

        return false;
    }


    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            Entity e = GetComponent<Entity>();
            if (e.InnerInit())
            {
                BaseEntity.Attach(e.Modifiers);
            }

            return true;
        }

        return false;
    }

    protected override void InnerDeInitialize()
    {
        BaseEntity.Yeet();

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        BaseEntity = null;

        base.DestroyVars();
    }

    protected override bool PostEnable()
    {
        if (base.PostEnable())
        {
            if (!BaseEntity.GetObj(0).enabled)
            {
                enabled = false;
                return false;
            }

            return true;
        }

        return false;
    }
}
