using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Body), typeof(SpeedLimit), typeof(ApplyFriction))]
public class PermitMovement : AbstractPermission<PermitMovement, CommandMovement, Body, Controller>
{
    public float acceleration = 0f;
    public float airborneAcceleration = 0f;
    public float globalSpeedLimit = 0f;
    public Vector3 axisSpeedLimits = Vector3.zero;
    public bool tilt = true;

    SpeedLimit SP;
    ApplyFriction FR;
    Vector3 direction = Vector3.zero;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            SP = GetComponent<SpeedLimit>();
            FR = GetComponent<ApplyFriction>();

            return true;
        }
        return false;
    }

    protected override void DestroyVars()
    {
        SP = null;
        FR = null;
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
        Quaternion groundtransform = transform.rotation;
        if (tilt)
            groundtransform = SP.GC.groundAngle * groundtransform;

        if (SP.GC.Grounded)
        {
            FR.Slow();
        }

        SpecificActor.Rb.AddForce(groundtransform *
            SP.ForceAdjustment(direction * (SP.GC.Grounded ? acceleration : airborneAcceleration), tilt, globalSpeedLimit, axisSpeedLimits.x, axisSpeedLimits.y, axisSpeedLimits.z));
        //SpecificActor.IDENT_VELOCITY = Mathf.Sqrt(SpecificActor.Rb.velocity.x * SpecificActor.Rb.velocity.x + SpecificActor.Rb.velocity.z * SpecificActor.Rb.velocity.z);

        direction = Vector3.zero;
    }
}
