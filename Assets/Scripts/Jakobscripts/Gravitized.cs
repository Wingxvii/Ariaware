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
    public ForcePointer VP;
    public float maxBias = 0.001f;
    public float FPradius = 2.00f;
    public bool showForces = true;
    public bool showVelocities = true;
    MeshRenderer FPmesh;
    MeshRenderer VPmesh;
    Transform suggestedParent;

    Quaternion motionDetect;

    Vector3 pInitial;
    Vector3 pTravel;

    void Awake()
    {
        motionDetect = new Quaternion();
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

    public void suggestParent(Transform ATM)
    {
        if (suggestedParent == null)
        {
            suggestedParent = ATM;
            transform.SetParent(ATM);
            if (motionDetect != null)
            {
                motionDetect = Quaternion.Inverse(ATM.transform.rotation);
            }
        }
        else if ((suggestedParent.position - transform.position).magnitude > (transform.position - ATM.position).magnitude)
        {
            suggestedParent = ATM;
            transform.SetParent(ATM);
            if (motionDetect != null)
            {
                motionDetect = Quaternion.Inverse(ATM.transform.rotation);
            }
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
        if (VP != null)
        {
            VP.SetRadius(FPradius);
            VP.SetForceLoc(rb.velocity);
            VP.SetBasePosition(transform.position);
            VP.UpdateLocation();
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

    public void enableVelocityPointer(bool enable)
    {
        if (VP != null)
        {
            VPmesh.enabled = enable;
        }
    }

    public void LateUpdate()
    {
        if (suggestedParent != null)
        {
            rb.velocity = suggestedParent.rotation * motionDetect * rb.velocity;
            rb.angularVelocity = suggestedParent.rotation * motionDetect * rb.angularVelocity;
            Debug.Log(suggestedParent.rotation * motionDetect);
            suggestedParent = null;
        }
    }

    public enum eType
    {
        player,
        enemy,
        other
    }
}
