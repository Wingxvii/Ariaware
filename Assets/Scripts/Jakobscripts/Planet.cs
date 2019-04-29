using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    GravityPoint[] gPoints;
    Rigidbody rb;
    static float Threshold = 0.001f;
    public bool isActive = true;

    public planetName pName = planetName.mainPlanet;

    [SerializeField]
    public Gravitized.eType[] attracts;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        gPoints = transform.GetComponentsInChildren<GravityPoint>();
        foreach (GravityPoint GP in gPoints)
        {
            Debug.Log("I'm here");
            GP.setAOE(GP.oppositeEq(Threshold / (rb.mass * GP.pullScale)));
        }
    }

    void Update()
    {
        
    }

    public void pull(Gravitized G)
    {
        foreach (GravityPoint GP in gPoints)
        {
            Vector3 toG = GP.transform.position - G.transform.position;
            float magG = toG.magnitude;
            if (GP.areaOfEffect > magG && !(GP.radialCutOff > 0 && GP.radialCutOff < magG))
                G.GetComponent<Rigidbody>().AddForce(toG * rb.mass * GP.pullMag(magG) * GP.pullScale);
        }
    }

    public enum planetName
    {
        mainPlanet,
        nextPlanet
    }
}
