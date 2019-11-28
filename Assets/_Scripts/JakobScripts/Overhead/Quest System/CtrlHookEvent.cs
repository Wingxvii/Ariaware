using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlHookEvent : QuestEvent<CtrlHookEvent, CtrlHookListener>
{
    public Entity entityRef = null;

    public CtrlHookEvent(Entity toAttach) : base(true)
    {
        entityRef = toAttach;
    }
}
