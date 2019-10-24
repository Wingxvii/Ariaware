using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody), typeof(GroundCheck))]
public class SpeedLimit : Modifier
{
    public GroundCheck GC;

    Rigidbody rb;

    //public float globalClampValue = 0f;
    //public Vector3 axisUpperBound = Vector3.zero;
    //public Vector3 axisLowerBound = Vector3.zero;
    public ClampType x = ClampType.None;
    public ClampType y = ClampType.None;
    public ClampType z = ClampType.None;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            rb = GetComponent<Rigidbody>();
            GC = GetComponent<GroundCheck>();

            return true;
        }

        return false;
    }

    protected override void DestroyVars()
    {
        GC = null;
        rb = null;

        base.DestroyVars();
    }

    float SquareLengthCalculation(Vector3 vel)
    {
        return
            (x == ClampType.Global ? vel.x * vel.x : 0) +
            (y == ClampType.Global ? vel.y * vel.y : 0) +
            (z == ClampType.Global ? vel.z * vel.z : 0);
    }

    public Vector3 ForceAdjustment(Vector3 force, bool tilt, float globalLimit = 0, float individualX = 0, float individualY = 0, float individualZ = 0)
    {
        Vector3 combined = new Vector3(individualX, individualY, individualZ);

        Vector3 vel = rb.velocity;
        if (tilt)
            vel = Quaternion.Inverse(GC.groundAngle) * vel;
        vel = Quaternion.Inverse(rb.transform.rotation) * vel;

        Vector3 afterVel = vel + force / rb.mass * Time.fixedDeltaTime;

        float globalLength = Mathf.Sqrt(SquareLengthCalculation(afterVel));
        float preLength = Mathf.Sqrt(SquareLengthCalculation(vel));

        afterVel.x = GetClamped(x, afterVel.x, globalLength, Mathf.Max(globalLimit, preLength), Mathf.Max(individualX, Mathf.Abs(vel.x)));
        afterVel.y = GetClamped(y, afterVel.y, globalLength, Mathf.Max(globalLimit, preLength), Mathf.Max(individualY, Mathf.Abs(vel.y)));
        afterVel.z = GetClamped(z, afterVel.z, globalLength, Mathf.Max(globalLimit, preLength), Mathf.Max(individualZ, Mathf.Abs(vel.z)));

        return (afterVel - vel) * rb.mass / Time.fixedDeltaTime;
    }

    float GetClamped(ClampType axis, float velocityAxis, float globalLength, float globalLimit, float individualLimit)
    {
        if (axis == ClampType.Global && globalLength > globalLimit)
            return velocityAxis / globalLength * globalLimit;
        else if (axis == ClampType.Singular)
            return Mathf.Clamp(velocityAxis, -individualLimit, individualLimit);
        return velocityAxis;
    }
}

public enum ClampType
{
    None,
    Global,
    Singular
}