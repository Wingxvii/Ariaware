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
    //bool canJump = false;

    SpeedLimit SP;

    //List<Collision> collisions;
    //protected List<Collision> Collisions
    //{
    //    get { Init(); return collisions; }
    //    private set { Init(); collisions = value; }
    //}

    protected override void CreateVars()
    {
        base.CreateVars();

        SP = GetComponent<SpeedLimit>();
        //Collisions = new List<Collision>();
    }

    protected override void DestroyVars()
    {
        //Collisions = null;
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
            cooldown = true;

        jump = false;
        //canJump = false;
    }

    //void CullCollisions()
    //{
    //    canJump = false;
    //    for (int i = Collisions.Count - 1; i >= 0; i--)
    //    {
    //        if (CullSingle(Collisions[i]))
    //        {
    //            canJump = true;
    //            i = -1;
    //        }
    //    }
    //}

    //bool CullSingle(Collision collision)
    //{
    //    Vector3 norm = Vector3.zero;
    //
    //    for (int j = collision.contactCount - 1; j >= 0; j--)
    //        norm += collision.GetContact(j).normal;
    //
    //    if (norm.sqrMagnitude > 0)
    //    {
    //        Vector3.Normalize(norm);
    //        if (Vector3.Dot(norm, Vector3.up) > Mathf.Cos(maxAngle * Mathf.Deg2Rad))
    //            return true;
    //    }
    //    return false;
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //if (!Collisions.Contains(collision))
    //    //    Collisions.Add(collision);
    //    //if (!canJump)
    //    //{
    //    //    if (CullSingle(collision))
    //    //        canJump = true;
    //    //}
    //}

    //private void OnCollisionStay(Collision collision)
    //{
    //    if (!canJump)
    //    {
    //        if (CullSingle(collision))
    //            canJump = true;
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    //Collisions.Remove(collision);
    //}
}
