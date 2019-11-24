using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Body))]
public class ApplyFriction : Modifier
{
    Body theBody;
    Rigidbody rb;

    public float strength = 10f;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            theBody = GetComponent<Body>();

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            rb = theBody.Rb;

            Collider[] cols = theBody.Col;
            for (int i = 0; i < cols.Length; ++i)
            {
                cols[i].material.frictionCombine = PhysicMaterialCombine.Minimum;
                cols[i].material.dynamicFriction = 0f;
                cols[i].material.staticFriction = 0f;
            }

            return true;
        }

        return false;
    }

    protected override void InnerDeInitialize()
    {


        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        theBody = null;
        rb = null;

        base.DestroyVars();
    }

    public void Slow()
    {
        if (rb.velocity.sqrMagnitude > strength * strength)
        {
            rb.velocity -= rb.velocity.normalized * strength;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }
}
