using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class GroundCheck : ObjectStat
{
    public bool tilt = true;

    public Quaternion groundAngle { get; private set; } = Quaternion.identity;

    public bool Grounded { get; private set; } = false;
    bool backCheckGrounded = false;
    public float maxAngle = 0f;

    public bool DebugShit = false;

    protected override void UpdateData()
    {
        backCheckGrounded = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isActiveAndEnabled)
        {
            if (backCheckGrounded)
            {
                backCheckGrounded = false;
                Grounded = false;
                groundAngle = Quaternion.identity;
            }

            if (CullSingle(collision))
                Grounded = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //if (DebugShit)
        //    Debug.Log(collision.collider.gameObject.name);
        if (isActiveAndEnabled)
        {
            if (backCheckGrounded)
            {
                backCheckGrounded = false;
                Grounded = false;
                groundAngle = Quaternion.identity;
            }

            if (CullSingle(collision))
                Grounded = true;
        }

        if (DebugShit)
        {
            Debug.Log(collision.collider.gameObject.name);
            Debug.Log(groundAngle.eulerAngles);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isActiveAndEnabled)
        {
            if (backCheckGrounded)
            {
                backCheckGrounded = false;
                Grounded = false;
                groundAngle = Quaternion.identity;
            }
        }
    }

    bool CullSingle(Collision collision)
    {
        Vector3 norm = Vector3.zero;

        for (int j = collision.contactCount - 1; j >= 0; j--)
            norm += collision.GetContact(j).normal;

        if (norm.sqrMagnitude > 0)
        {
            Vector3.Normalize(norm);
            float DotAngle = Vector3.Dot(norm, Ent.GetObj(0).transform.up);
            if (DotAngle > Mathf.Cos(maxAngle * Mathf.Deg2Rad))
            {
                if (tilt && (!Grounded || DotAngle > Vector3.Dot(groundAngle * Ent.GetObj(0).transform.up, Ent.GetObj(0).transform.up)))
                {
                    groundAngle = Quaternion.FromToRotation(Ent.GetObj(0).transform.up, norm);
                }
                return true;
            }
        }
        return false;
    }
}
