using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody), typeof(GroundCheck))]
public class SpeedLimit : Modifier
{
    GroundCheck gC;
    public GroundCheck GC
    {
        get { return gC; }
        private set { gC = value; }
    }

    Rigidbody rb;

    //public float globalClampValue = 0f;
    //public Vector3 axisUpperBound = Vector3.zero;
    //public Vector3 axisLowerBound = Vector3.zero;
    public ClampType x = ClampType.None;
    public ClampType y = ClampType.None;
    public ClampType z = ClampType.None;

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        rb = GetComponent<Rigidbody>();
        GC = GetComponent<GroundCheck>();
    }

    protected override void DeInitialize()
    {


        base.DeInitialize();
    }

    protected override void DestroyVars()
    {
        GC = null;
        rb = null;

        base.DestroyVars();
    }

    private void FixedUpdate()
    {
        //ApplyFixedUpdateModifier();
    }

    //public void ApplyFixedUpdateModifier()
    //{
    //    Vector3 vel = Quaternion.Inverse(GC.groundAngle) * rb.velocity;
    //    float globalLength = Mathf.Sqrt(
    //        SquareLengthCalculation(vel)
    //    );
    //
    //    if (x == ClampType.Global && globalLength > globalClampValue)
    //        vel.x = vel.x / globalLength * globalClampValue;
    //    else if (x == ClampType.Singular)
    //        vel.x = Mathf.Clamp(vel.x, axisLowerBound.x, axisUpperBound.x);
    //
    //    if (y == ClampType.Global && globalLength > globalClampValue)
    //        vel.y = vel.y / globalLength * globalClampValue;
    //    else if (y == ClampType.Singular)
    //        vel.y = Mathf.Clamp(vel.y, axisLowerBound.y, axisUpperBound.y);
    //
    //    if (z == ClampType.Global && globalLength > globalClampValue)
    //        vel.z = vel.z / globalLength * globalClampValue;
    //    else if (z == ClampType.Singular)
    //        vel.z = Mathf.Clamp(vel.z, axisLowerBound.z, axisUpperBound.z);
    //
    //    rb.velocity = GC.groundAngle * vel;
    //}

    float SquareLengthCalculation(Vector3 vel)
    {
        return
            (x == ClampType.Global ? vel.x * vel.x : 0) +
            (y == ClampType.Global ? vel.y * vel.y : 0) +
            (z == ClampType.Global ? vel.z * vel.z : 0);
    }


    //public Vector3 AdjustedForce(Vector3 force, bool tilt, float globalLimit = 0, float individualX = 0, float individualY = 0, float individualZ = 0)
    //{
    //    Vector3 combined = new Vector3(individualX, individualY, individualZ);
    //
    //    Vector3 vel = rb.velocity;
    //    Vector3 afterVel = vel + force / rb.mass * Time.fixedDeltaTime;
    //    if (tilt)
    //    {
    //        vel = Quaternion.Inverse(GC.groundAngle) * vel;
    //        afterVel = Quaternion.Inverse(GC.groundAngle) * afterVel;
    //    }
    //
    //    float globalLength = Mathf.Sqrt(SquareLengthCalculation(afterVel));
    //
    //    afterVel.x = GetClamped(x, afterVel.x, globalLength, globalLimit, individualX);
    //    afterVel.y = GetClamped(y, afterVel.y, globalLength, globalLimit, individualY);
    //    afterVel.z = GetClamped(z, afterVel.z, globalLength, globalLimit, individualZ);
    //
    //   return GC.groundAngle * (afterVel - vel) * rb.mass / Time.fixedDeltaTime;
    //}

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