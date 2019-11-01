using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class AirResistance : Modifier
{
    Rigidbody rb;

    public float globalResistance = 0f;
    public Vector3 axisResistance = Vector3.zero;
    public ClampType x = ClampType.None;
    public ClampType y = ClampType.None;
    public ClampType z = ClampType.None;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddFixedUpdate();

            rb = GetComponent<Rigidbody>();

            return true;
        }

        return false;
    }

    protected override void DestroyVars()
    {
        rb = null;

        base.DestroyVars();
    }

    protected override void FixedUpdateObject()
    {
        ApplyFixedUpdateModifier();
    }

    public void ApplyFixedUpdateModifier()
    {
        Vector3 vel = rb.velocity;

        rb.AddForce(new Vector3(
            x == ClampType.Global ? -vel.x : 0,
            y == ClampType.Global ? -vel.y : 0,
            z == ClampType.Global ? -vel.z : 0
        ) * globalResistance
        
        + new Vector3(
            x == ClampType.Global ? -vel.x * axisResistance.x : 0,
            y == ClampType.Global ? -vel.y * axisResistance.y : 0,
            z == ClampType.Global ? -vel.z * axisResistance.z : 0
        ));
    }
}