using UnityEngine;

[DisallowMultipleComponent]
public class Gravitized : MonoBehaviour
{
    public bool byAll = false;
    public bool onlyStrongestPull = false;
    public eType typeOfG = eType.other;
    Rigidbody rb;
    Vector3 totalForces = Vector3.zero;
    float radialDistance = 0f;
    public ForcePointer FP;
    public float maxBias = 0.001f;
    public float FPradius = 2.00f;
    public bool showForces = true;
    MeshRenderer FPmesh;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (FP != null)
        {
            FP = Instantiate(FP);
            FP.SetBasePosition(transform.position);
            FP.GetComponent<Collider>().enabled = false;
            FPmesh = FP.GetComponent<MeshRenderer>();
        }
    }

    public void NewForce(Vector3 force, float RadialDistance)
    {
        if (onlyStrongestPull)
        {
            if (force.magnitude > totalForces.magnitude + maxBias)
            {
                totalForces = force;
                radialDistance = RadialDistance;
            }
            else if (Mathf.Abs(force.magnitude - totalForces.magnitude) <= maxBias && RadialDistance < radialDistance)
            {
                totalForces = force;
                radialDistance = RadialDistance;
            }
        }
        else
        {
            totalForces += force;
        }
    }

    public void ApplyForces()
    {
        rb.AddForce(totalForces);
        if (FP != null)
        {
            FP.SetRadius(FPradius);
            FP.SetForceLoc(totalForces);
            FP.SetBasePosition(transform.position);
            FP.UpdateLocation();
        }
        ClearForces();
    }

    public void ClearForces()
    {
        totalForces = Vector3.zero;
        radialDistance = 0f;
    }

    public void enableForcePointer(bool enable)
    {
        if (FP != null)
        {
            FPmesh.enabled = enable;
        }
    }

    public enum eType
    {
        player,
        enemy,
        other
    }
}
