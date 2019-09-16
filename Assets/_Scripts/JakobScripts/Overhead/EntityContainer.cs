using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

//[DisallowMultipleComponent]
public class EntityContainer : BasePACES
{
    JoinedList<EntityContainer, Entity> objectList;
    public JoinedList<EntityContainer, Entity> ObjectList
    {
        get { return objectList; }
        private set { objectList = value; }
    }

    JoinedList<EntityContainer, SlotBase> slots;
    public JoinedList<EntityContainer, SlotBase> Slots
    {
        get { return slots; }
        protected set { slots = value; }
    }

    protected override void Initialize()
    {
        //transform.position = Vector3.zero;
        //transform.rotation = Quaternion.identity;

        base.Initialize();

        SetList();
    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();

        SetSlots();
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        Slots = new JoinedList<EntityContainer, SlotBase>(this);
        ObjectList = new JoinedList<EntityContainer, Entity>(this);
    }

    protected override void DeInitialize()
    {
        ObjectList.Yeet();

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        Slots.Yeet();

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        ObjectList = null;
        Slots = null;

        base.DestroyVars();
    }

    protected void SetList()
    {
        Entity[] ents = GetComponentsInChildren<Entity>();
        for (int i = ents.Length - 1; i >= 0; i--)
        {
            if (ents[i].isActiveAndEnabled)
            {
                ents[i].Init();
                ents[i].InnerInit();
                ents[i].AttachContainerAndSlot();
                //ObjectList.Attach(ents[i].Container);
            }
        }
    }

    protected void SetSlots()
    {
        SlotBase[] s = GetComponents<SlotBase>();
        for (int i = 0; i < s.Length; i++)
        {
            s[i].Init();
            Slots.Attach(s[i].Container);
        }
    }

    protected override void PostDisable()
    {
        if (!enabled)
        {
            for (int i = Slots.Joins.Count - 1; i >= 0; --i)
                Slots.GetObj(i).enabled = false;
        }

        base.PostDisable();
    }

    //private void Update()
    //{
    //    //if (Input.GetKey(KeyCode.Equals) && SceneManager.GetActiveScene().name == "SampleScene")
    //    //    SceneManager.LoadScene(1);
    //}
}
