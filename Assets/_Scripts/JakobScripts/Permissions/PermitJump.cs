using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Body), typeof(SpeedLimit))]
public class PermitJump : AbstractPermission<PermitJump, CommandJump, Body, Controller>
{
    public float jumpForce = 0f;
    public float globalSpeedLimit = 0f;
    public Vector3 axisSpeedLimits = Vector3.zero;
    public bool tilt = true;
    bool cooldown = false;

    bool jump = false;
    Animator anim;

    SpeedLimit SP;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            SP = GetComponent<SpeedLimit>();
            anim = GetComponent<Animator>();

            return true;
        }

        return false;
    }

    protected override void DestroyVars()
    {
        SP = null;

        base.DestroyVars();
    }

    public void Jump()
    {
        jump = true;
    }

    protected override void FeedPuppet()
    {
        if (jump && SP.GC.Grounded && cooldown)
        {
            SpecificActor.Container.GetObj(0).pState |= (uint)PlayerState.Jumping;

            Quaternion groundtransform = transform.rotation;
            if (tilt)
                groundtransform = SP.GC.groundAngle * groundtransform;

            Vector3 v = Quaternion.Inverse(groundtransform) * SpecificActor.Rb.velocity;
            v.y = 0f;
            SpecificActor.Rb.velocity = groundtransform * v;

            SpecificActor.Rb.AddForce(groundtransform *
                SP.ForceAdjustment(Vector3.up * jumpForce, tilt, globalSpeedLimit, axisSpeedLimits.x, axisSpeedLimits.y, axisSpeedLimits.z));
            cooldown = false;
        }
        else if (!cooldown)
        {
            SpecificActor.Container.GetObj(0).pState &= ~(uint)PlayerState.Jumping;
            cooldown = true;
        }

        jump = false;
    }
}
