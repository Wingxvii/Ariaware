using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Puppet))]
public class PlayerSourceListener : QuestListener<PlayerSourceListener, PlayerSourceEvent>
{
    public BindVec3 bindPos;
    public BindVec3 bindRot;
    public bool bindState = false;

    Puppet p;
    EntityContainer ec;
    uint pState;

    protected override void Awake()
    {
        base.Awake();
        p = GetComponent<Puppet>();
        ec = p.Container.GetObj(0);
    }

    protected void UseEvent(PlayerSourceEvent questEvent)
    {
        if (ec != null)
            pState = ec.pState;

        questEvent.WPD.sendPos = VectorSplit(transform.position, questEvent.WPD.sendPos, bindPos);
        questEvent.WPD.sendPos = VectorSplit(transform.position, questEvent.WPD.sendPos, bindPos);
        questEvent.WPD.sendState = bindState ? pState : questEvent.WPD.sendState;
    }

    Vector3 VectorSplit(Vector3 v1, Vector3 v2, BindVec3 bv)
    {
        return new Vector3(bv.x ? v1.x : v2.x, bv.y ? v1.y : v2.y, bv.z ? v1.z : v2.z);
    }
}
