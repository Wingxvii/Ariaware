using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Planet : MonoBehaviour
{
    GravityPoint[] gPoints;
    Rigidbody rb;
    static float Threshold = 0.001f;
    public bool isActive = true;
    public bool showAtmosphere = true;

    public Vector3 rotationAxis = Vector3.zero;
    public float degreesPerSecond = 0.0f;

    public planetName pName = planetName.mainPlanet;

    [SerializeField]
    public Gravitized.eType[] attracts;

    public Atmosphere[] atmosphere;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
    }

    void FixedUpdate()
    {
        if (rotationAxis.magnitude > 0)
        {
            transform.rotation = Quaternion.AngleAxis(degreesPerSecond * Time.fixedDeltaTime,
                rotationAxis.normalized) * transform.rotation;
        }
    }

    public void pull(Gravitized G)
    {
        if (rb != null)
        {
            foreach (GravityPoint GP in gPoints)
            {
                Vector3 toG = GP.transform.position - G.transform.position;
                float magG = toG.magnitude;
                if (magG > 0)
                    toG.Normalize();
                if (GP.areaOfEffect > magG && !(GP.radialCutOff > 0 && GP.radialCutOff < magG))
                    G.NewForce(toG * rb.mass * GP.pullMag(magG) * GP.pullScale, magG);
            }
        }

        Ray ray = new Ray();

        foreach(Atmosphere ATM in atmosphere)
        {
            if (ATM.moves(G, ray, this))
            {
                break;
            }
        }
    }

    public enum planetName
    {
        mainPlanet,
        nextPlanet
    }
}
