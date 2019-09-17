using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlotList<T> : SlotBase where T : Entity
{
    JoinedList<SlotBase, Entity> objectList;
    public JoinedList<SlotBase, Entity> ObjectList
    {
        get { return objectList; }
        private set { objectList = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();

        //SetList();
    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();


    }

    protected override void CreateVars()
    {
        base.CreateVars();

        ObjectList = new JoinedList<SlotBase, Entity>(this);
        ObjectBase = ObjectList;
    }

    protected virtual void SetList()
    {
        for (int i = Container.GetObj(0).ObjectList.Joins.Count - 1; i >= 0; i--)
        {
            T obj = EType<T>.FindType(Container.GetObj(0).ObjectList.GetObj(i));
            if (obj != null)
                ObjectList.Attach(obj.EntitySlot);
        }
    }

    protected override void DeInitialize()
    {
        ObjectList.Yeet();

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {


        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        ObjectList = null;

        base.DestroyVars();
    }
}
