using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class Gravitized : MonoBehaviour
{
    public bool byAll = false;
    public bool onlyStrongestPull = false;
    public eType typeOfG = eType.other;
    Rigidbody rb;
    protected Vector3 totalForces = Vector3.zero;
    float radialDistance = 0f;
    public ForcePointer FP;
    public ForcePointer VP;
    public float maxBias = 0.001f;
    public float FPradius = 2.00f;
    public bool showForces = true;
    public bool showVelocities = true;
    MeshRenderer FPmesh;
    MeshRenderer VPmesh;
    public bool freeze = false;

    Vector3 atmosphereForce = Vector3.zero;
    Vector3 atmosphereRadialForce = Vector3.zero;
    //Planet suggestedParent;

    //Quaternion motionDetect;

    //Vector3 pInitial;
    //Vector3 pTravel;

    void Awake()
    {
        Initialize();
    }

    protected void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        if (FP != null)
        {
            FP = Instantiate(FP);
            FP.SetBasePosition(transform.position);
            FP.GetComponent<Collider>().enabled = false;
            FPmesh = FP.GetComponent<MeshRenderer>();
        }
        if (VP != null)
        {
            VP = Instantiate(FP);
            VP.SetBasePosition(transform.position);
            VP.GetComponent<Collider>().enabled = false;
            VPmesh = VP.GetComponent<MeshRenderer>();
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

    public Rigidbody GetRB()
    {
        return rb;
    }

    public void AddAtmosphereForce(Planet ATM)
    {

    }

    public void ApplyForces()
    {
        if (!freeze)
        {
            rb.AddForce(totalForces);
            if (FP != null)
            {
                FP.SetRadius(FPradius);
                FP.SetForceLoc(totalForces);
                FP.SetBasePosition(transform.position);
                FP.UpdateLocation();
            }
            if (VP != null)
            {
                VP.SetRadius(FPradius);
                VP.SetForceLoc(rb.velocity);
                VP.SetBasePosition(transform.position);
                VP.UpdateLocation();
            }
        }
        //ClearForces();
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

    public void enableVelocityPointer(bool enable)
    {
        if (VP != null)
        {
            VPmesh.enabled = enable;
        }
    }

    //public void LateUpdate()
    //{
    //    if (suggestedParent != null)
    //    {
    //        rb.velocity = suggestedParent.rotation * motionDetect * rb.velocity;
    //        rb.angularVelocity = suggestedParent.rotation * motionDetect * rb.angularVelocity;
    //        Debug.Log(suggestedParent.rotation * motionDetect);
    //        suggestedParent = null;
    //    }
    //}

    public enum eType
    {
        player,
        enemy,
        other
    }
}
