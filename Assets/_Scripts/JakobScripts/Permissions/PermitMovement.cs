using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Body), typeof(SpeedLimit))]
public class PermitMovement : AbstractPermission<PermitMovement, CommandMovement, Body, Controller>
{
    public float acceleration = 0f;
    public float airborneAcceleration = 0f;
    public float globalSpeedLimit = 0f;
    public Vector3 axisSpeedLimits = Vector3.zero;
    public bool tilt = true;

    SpeedLimit SP;
    Vector3 direction = Vector3.zero;

    protected override void CreateVars()
    {
        base.CreateVars();

        SP = GetComponent<SpeedLimit>();
    }

    protected override void DestroyVars()
    {
        SP = null;
        base.DestroyVars();
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
        //direction.y = 0;
        if (direction.sqrMagnitude > 0)
            direction = direction.normalized;
    }

    protected override void FeedPuppet()
    {
        //SpecificActor.Rb.AddForce(SP.GC.groundAngle * SpecificActor.transform.rotation * 
        //    direction * (SP.GC.Grounded ? acceleration : airborneAcceleration));
        Quaternion groundtransform = transform.rotation;
        if (tilt)
            groundtransform = SP.GC.groundAngle * groundtransform;

        SpecificActor.Rb.AddForce(groundtransform *
            SP.ForceAdjustment(direction * (SP.GC.Grounded ? acceleration : airborneAcceleration), tilt, globalSpeedLimit, axisSpeedLimits.x, axisSpeedLimits.y, axisSpeedLimits.z));

        direction = Vector3.zero;
    }
}
