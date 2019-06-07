using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlanetManager : MonoBehaviour
{
    //Planet[] planets;
    //Gravitized[] affectedByGravity;
    //Atmosphere[] atmosphere;

    List<Planet> planets;
    List<Gravitized> affectedByGravity;
    //List<Atmosphere> atmosphere;

    public bool gravityEnabled = true;
    public bool showForces = false;
    public bool showAtmosphere = true;
    public bool showVelocities = false;

    void Awake()
    {
        planets = new List<Planet>();
        affectedByGravity = new List<Gravitized>();
        //atmosphere = new List<Atmosphere>();

        Planet[] sub = FindObjectsOfType<Planet>();
        Gravitized[] sub2 = FindObjectsOfType<Gravitized>();

        for(int i = 0; i < sub.Length; i++)
        {
            planets.Add(sub[i]);
        }
        for (int i = 0; i < sub2.Length; i++)
        {
            affectedByGravity.Add(sub2[i]);
        }
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        if (gravityEnabled)
        {
            planetaryPull();
        }
    }

    void planetaryPull()
    {
        for(int i = 0; i < affectedByGravity.Count; i++)
        {
            affectedByGravity[i].transform.SetParent(null);
            affectedByGravity[i].ClearForces();
        }

        for(int i = 0; i < planets.Count; i++)
        {
            CalcGravity(planets[i]);
        }

        for (int i = 0; i < affectedByGravity.Count; i++)
        {
            ApplyForces(affectedByGravity[i]);
        }

        for (int i = 0; i < planets.Count; i++)
        {
            planets[i].UpdatePositions();
        }
    }

    private void CalcGravity(Planet p)
    {
        if (p.isActive)
        {
            for (int i = 0; i < p.atmosphere.Length; i++)
            {
                EnableAtmosphere(p, p.atmosphere[i]);
            }

            for (int i = 0; i < affectedByGravity.Count; i++)
            {
                PullGravityAfflictedObjects(p, affectedByGravity[i]);
            }
        }
    }

    private void EnableAtmosphere(Planet p, Atmosphere ATM)
    {
        if (showAtmosphere && p.showAtmosphere)
        {
            ATM.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            ATM.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void PullGravityAfflictedObjects(Planet p, Gravitized g)
    {
        if (g.byAll)
        {
            p.pull(g);
        }
        else
        {
            for (int i = 0; i < p.attracts.Length; i++)
            {
                CheckTypeAndPull(p, g, p.attracts[i]);
            }
        }
    }

    private void CheckTypeAndPull(Planet p, Gravitized g, Gravitized.eType ET)
    {
        if (ET == g.typeOfG)
        {
            p.pull(g);
        }
    }

    private void ApplyForces(Gravitized g)
    {
        g.ApplyForces();
        g.enableForcePointer(showForces && g.showForces);
        g.enableVelocityPointer(showVelocities && g.showVelocities);
    }
}
