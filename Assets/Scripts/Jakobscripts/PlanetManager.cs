using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlanetManager : MonoBehaviour
{
    Planet[] planets;
    Gravitized[] affectedByGravity;
    Atmosphere[] atmosphere;
    public bool gravityEnabled = true;
    public bool showForces = false;
    public bool showAtmosphere = true;
    public bool showVelocities = false;

    void Awake()
    {
        planets = FindObjectsOfType<Planet>();
        affectedByGravity = FindObjectsOfType<Gravitized>();
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
        foreach (Gravitized g in affectedByGravity)
        {
            g.transform.SetParent(null);
        }

        foreach (Planet p in planets)
        {
            if (p.isActive)
            {
                foreach (Atmosphere ATM in p.atmosphere)
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

                foreach (Gravitized g in affectedByGravity)
                {
                    if (g.byAll)
                    {
                        p.pull(g);
                    }
                    else
                    {
                        foreach (Gravitized.eType ET in p.attracts)
                        {
                            if (ET == g.typeOfG)
                            {
                                p.pull(g);
                            }
                        }
                    }
                }
            }
        }

        foreach (Gravitized g in affectedByGravity)
        {
            g.ApplyForces();
            g.enableForcePointer(showForces && g.showForces);
            g.enableVelocityPointer(showVelocities && g.showVelocities);
        }
    }
}
