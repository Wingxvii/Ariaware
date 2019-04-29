using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    Planet[] planets;
    Gravitized[] affectedByGravity;

    void Awake()
    {
        planets = FindObjectsOfType<Planet>();
        affectedByGravity = FindObjectsOfType<Gravitized>();
    }

    void FixedUpdate()
    {
        planetaryPull();
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
    }
}
