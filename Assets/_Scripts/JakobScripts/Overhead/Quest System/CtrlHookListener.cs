using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlHookListener : QuestListener<CtrlHookListener, CtrlHookEvent>
{
    protected override void Awake()
    {
        base.Awake();
        AddFunction(AttachTo);
    }

    protected void AttachTo(CtrlHookEvent questEvent)
    {
        if (questEvent.entityRef != null)
            transform.SetParent(questEvent.entityRef.transform.parent, false);
    }
}
