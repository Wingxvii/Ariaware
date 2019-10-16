using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public abstract class ObjectStat : InitializableObject
{
    public JoinedVar<ObjectStat, Entity> Ent;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            Ent = new JoinedVar<ObjectStat, Entity>(this, false);

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
                Ent.Attach(e.JoinedStats);
            }

            return true;
        }

        return false;
    }

    protected override void InnerDeInitialize()
    {
        Ent.Yeet();

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        Ent = null;

        base.DestroyVars();
    }

    private void FixedUpdate()
    {
        UpdateData();
    }

    protected virtual void UpdateData() { }

    protected override bool PostEnable()
    {
        if (base.PostEnable())
        {
            if (!Ent.GetObj(0).enabled)
            {
                enabled = false;
                return false;
            }

            return true;
        }

        return false;
    }
}
