using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO - Switch Initialize and OnReparent to bools, such that things are not duplicated in the event that a reparent or awake fails
public abstract class Entity : BasePACES
{
    JoinedVar<Entity, SlotBase> entitySlot;
    public JoinedVar<Entity, SlotBase> EntitySlot
    {
        get { return entitySlot; }
        protected set { entitySlot = value; }
    }

    JoinedVar<Entity, EntityContainer> container;
    public JoinedVar<Entity, EntityContainer> Container
    {
        get { return container; }
        protected set { container = value; }
    }

    JoinedList<Entity, Modifier> modifiers;
    public JoinedList<Entity, Modifier> Modifiers
    {
        get { return modifiers; }
        protected set { modifiers = value; }
    }

    JoinedList<Entity, ObjectStat> joinedStats;
    public JoinedList<Entity, ObjectStat> JoinedStats
    {
        get { return joinedStats; }
        protected set { joinedStats = value; }
    }

    //JoinedList<Entity, Entity> entityChildren;
    //public JoinedList<Entity, Entity> EntityChildren
    //{
    //    get { return entityChildren; }
    //    protected set { entityChildren = value; }
    //}
    //
    //JoinedVar<Entity, Entity> entityParent;
    //public JoinedVar<Entity, Entity> EntityParent
    //{
    //    get { return entityParent; }
    //    protected set { entityParent = value; }
    //}

    protected override void Initialize()
    {
        base.Initialize();

        AttachContainerAndSlot();
    }

    public void AttachContainerAndSlot()
    {
        bool hasContainer = AttachContainer();
        if (AttachSlot(hasContainer))
        {
            //AttachEntityParent();
        }
    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();

        Modifier[] m = GetComponents<Modifier>();
        for (int i = 0; i < m.Length; i++)
        {
            m[i].Init();
            modifiers.Attach(m[i].BaseEntity);
        }

        ObjectStat[] o = GetComponents<ObjectStat>();
        for (int i = 0; i < o.Length; i++)
        {
            o[i].Init();
            joinedStats.Attach(o[i].Ent);
        }
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        Container = new JoinedVar<Entity, EntityContainer>(this, false);
        EntitySlot = new JoinedVar<Entity, SlotBase>(this, true);
        Modifiers = new JoinedList<Entity, Modifier>(this);
        JoinedStats = new JoinedList<Entity, ObjectStat>(this);
        //EntityChildren = new JoinedList<Entity, Entity>(this);
        //EntityParent = new JoinedVar<Entity, Entity>(this, true);
    }

    protected override void DeInitialize()
    {
        EntitySlot.Yeet();
        Container.Yeet();
        //EntityParent.Yeet();
        //EntityChildren.Yeet();

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        JoinedStats.Yeet();
        Modifiers.Yeet();

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        Modifiers = null;
        JoinedStats = null;
        EntitySlot = null;
        Container = null;
        //EntityParent = null;
        //EntityChildren = null;

        base.DestroyVars();
    }

    //private void AttachEntityParent()
    //{
    //    if (transform.parent != null)
    //    {
    //        BasePACES bp = transform.parent.GetComponent<BasePACES>();
    //        if (bp != null)
    //        {
    //            Entity ent = EType<Entity>.FindType(bp);
    //            if (ent != null)
    //            {
    //                EntityParent.Attach(ent.EntityChildren);
    //                return;
    //            }
    //            EntityParent.Yeet();
    //            return;
    //        }
    //        transform.SetParent(null);
    //        //Debug.Log("You should go!");
    //        //EntityParent.Yeet(true);
    //        return;
    //    }
    //    EntityParent.Yeet();
    //}

    private bool AttachContainer()
    {
        if (transform.parent != null)
        {
            //EntityContainer ec = transform.parent.GetComponent<EntityContainer>();
            EntityContainer ec = GetComponentInParent<EntityContainer>();
            if (ec != null && ec.AE)
            {
                ec.Init();
                ec.InnerInit();
                Container.Attach(ec.ObjectList);
                return true;
            }
        }
        Container.Yeet();
        return false;
    }

    private SlotBase ParseSlot(bool hasContainer)
    {
        if (hasContainer)
        {
            return GetSlot();
        }

        return null;
    }

    protected abstract SlotBase GetSlot();

    protected bool AttachSlot(bool hasContainer)
    {
        if (hasContainer)
        {
            SlotBase sb = ParseSlot(hasContainer);
            if (sb != null && sb.AE)
            {
                sb.Init();
                sb.InnerInit();
                EntitySlot.Attach(sb.ObjectBase);
                return true;
            }
            //else if (sb != null && !sb.isActiveAndEnabled)
            //    Debug.Log("OUTTAHERE: " + name);
            EntitySlot.Yeet(true);
            return false;
        }
        EntitySlot.Yeet();
        return true;
    }

    protected virtual void OnBeforeTransformParentChanged()
    {

    }

    protected void OnTransformParentChanged()
    {
        OnReparent();
    }

    //protected virtual void OnTransformParentChanged()
    //{
    //    AttachContainer();
    //    AttachSlot();
    //}

    protected virtual void PreReparent()
    {

    }

    protected virtual bool OnReparent()
    {
        Init();
        InnerInit();
        //base.OnReparent();

        bool hasContainer = AttachContainer();
        if (AttachSlot(hasContainer))
        {
            //AttachEntityParent();
            //Debug.Log(name);
            //for (int i = EntityChildren.Joins.Count - 1; i >= 0; i--)
            //{
            //    EntityChildren.GetObj(i).OnReparent();
            //}
            return true;
        }
        return false;
    }

    protected override void PostDisable()
    {
        if (!enabled)
        {
            for (int i = Modifiers.Joins.Count - 1; i >= 0; --i)
                Modifiers.GetObj(i).enabled = false;

            for (int i = JoinedStats.Joins.Count - 1; i >= 0; --i)
                JoinedStats.GetObj(i).enabled = false;
        }

        base.PostDisable();
    }

    //protected Transform GetRoot()
    //{
    //    if (transform.parent != null)
    //        return transform.parent.GetRoot();
    //}
}
