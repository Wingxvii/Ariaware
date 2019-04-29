using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlanetManager : MonoBehaviour
{
    Planet[] planets;
    Gravitized[] affectedByGravity;
    public bool gravityEnabled = true;
    public bool showForces = false;

    void Awake()
    {
        planets = FindObjectsOfType<Planet>();
        affectedByGravity = FindObjectsOfType<Gravitized>();
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
        foreach (Planet p in planets)
        {
            if (p.isActive)
            {
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
        }
    }
}
