using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Planet : MonoBehaviour
{
    GravityPoint[] gPoints;
    Rigidbody rb;
    GameObject pPhysics;
    Rigidbody physRB;
    static float Threshold = 0.001f;
    public bool isActive = true;
    public bool showAtmosphere = true;

    public Vector3 rotationAxis = Vector3.zero;
    public float degreesPerSecond = 0.0f;

    public planetName pName = planetName.mainPlanet;

    [SerializeField]
    public Gravitized.eType[] attracts;

    public Atmosphere[] atmosphere { get; private set; }

    Vector3 lastPosition, deltaPosition;
    Quaternion lastRotation, deltaRotation;
    Vector3 ax = Vector3.zero;
    float rot = 0f;

    void Awake()
    {
        atmosphere = GetComponentsInChildren<Atmosphere>();
        rb = GetComponent<Rigidbody>();
        SetPhysRB();
    }

    void SetPhysRB()
    {
        pPhysics = new GameObject();
        physRB = pPhysics.AddComponent<Rigidbody>();
        physRB.angularDrag = 0;
        physRB.drag = 0;
        physRB.useGravity = false;
        pPhysics.name = "Planet Physics";
        pPhysics.transform.SetParent(transform);
    }

    Rigidbody GetPhysRB()
    {
        return physRB;
    }

    void Start()
    {
        if (rb != null)
        {
            gPoints = transform.GetComponentsInChildren<GravityPoint>();
            foreach (GravityPoint GP in gPoints)
            {
                GP.setAOE(GP.oppositeEq(Threshold / (rb.mass * GP.pullScale)));
            }
            atmosphere = GetComponentsInChildren<Atmosphere>();
        }
        lastPosition = new Vector3();
        lastRotation = new Quaternion();
        deltaPosition = new Vector3();
        deltaRotation = new Quaternion();
    }

    void FixedUpdate()
    {
        if (rotationAxis.magnitude > 0)
        {
            transform.rotation = Quaternion.AngleAxis(degreesPerSecond * Time.fixedDeltaTime,
                rotationAxis.normalized) * transform.rotation;
        }
    }

    public void UpdatePositions()
    {
        deltaPosition = (transform.position - lastPosition) / Time.fixedDeltaTime;
        deltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);
        deltaRotation.ToAngleAxis(out rot, out ax);
        rot = rot / Time.fixedDeltaTime * Mathf.Deg2Rad;
        ax.Normalize();

        lastPosition = transform.position;
        lastRotation = transform.rotation;

        physRB.velocity = Vector3.zero;
        physRB.angularVelocity = Vector3.zero;
        pPhysics.transform.localPosition = Vector3.zero;
        pPhysics.transform.localRotation = Quaternion.identity;

        physRB.AddForce(deltaPosition);
        physRB.AddTorque(ax * rot);
    }

    public void pull(Gravitized G)
    {
        if (rb != null)
        {
            //foreach (GravityPoint GP in gPoints)
            for (int i = 0; i < gPoints.Length; i++)
            {
                ActivateGravity(G, gPoints[i]);
            }
        }

        Ray ray = new Ray();
        Rigidbody R = G.GetRB();
        Atmosphere currentATM = null;

        for (int i = 0; i < atmosphere.Length; i++)
        {
            if(GetInAtmosphere(G, atmosphere[i], ray))
            {
                if (currentATM != null)
                {
                    if (currentATM.GetDensity() < atmosphere[i].GetDensity())
                    {
                        currentATM = atmosphere[i];
                    }
                }
                else
                {
                    currentATM = atmosphere[i];
                }
            }
        }

        if (currentATM != null)
        {
            MoveByVelocity(R, currentATM);
            MoveByAngular(R, currentATM);
        }
    }

    private void MoveByVelocity(Rigidbody R, Atmosphere ATM)
    {
        Vector3 vByAngular = Vector3.Cross(physRB.angularVelocity, R.position - physRB.position);

        if ((physRB.velocity - R.velocity + vByAngular).magnitude >
                    ATM.GetDensity() / Time.fixedDeltaTime)
        {
            R.AddForce((physRB.velocity - R.velocity + vByAngular).normalized
                * ATM.GetDensity() / Time.fixedDeltaTime);
        }
        else
        {
            R.AddForce((physRB.velocity - R.velocity + vByAngular) / Time.fixedDeltaTime);
        }
    }

    private void MoveByAngular(Rigidbody R, Atmosphere ATM)
    {
        if ((physRB.angularVelocity - R.angularVelocity).magnitude >
                    ATM.GetDensity() / Time.fixedDeltaTime)
        {
            R.AddTorque((physRB.angularVelocity - R.angularVelocity).normalized
                * ATM.GetDensity() / Time.fixedDeltaTime);
        }
        else
        {
            R.AddTorque((physRB.angularVelocity - R.angularVelocity) / Time.fixedDeltaTime);
        }
    }

    private bool GetInAtmosphere(Gravitized G, Atmosphere ATM, Ray ray)
    {
        return ATM.moves(G, ray, this);
    }

    private void ActivateGravity(Gravitized G, GravityPoint GP)
    {
        Vector3 toG = GP.transform.position - G.transform.position;
        float magG = toG.magnitude;
        if (magG > 0)
            toG.Normalize();
        if (GP.areaOfEffect > magG && !(GP.radialCutOff > 0 && GP.radialCutOff < magG))
            G.NewForce(toG * rb.mass * GP.pullMag(magG) * GP.pullScale, magG);
    }

    public enum planetName
    {
        mainPlanet,
        nextPlanet
    }
}
