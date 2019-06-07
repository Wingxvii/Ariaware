using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    Collider[] col;
    public float thickness = 0.001f;
    //float control = 1.00f;

    void Awake()
    {
        col = GetComponents<Collider>();
        for (int i = 0; i < col.Length; i++)
        {
            col[i].enabled = false;
        }
    }

    public float GetDensity()
    {
        return thickness;
    }

    public bool moves(Gravitized G, Ray ray, Planet P)
    {
        ray.origin = G.transform.position;
        ray.direction = (transform.position - G.transform.position).normalized;

        for (int i = 0; i < col.Length; i++)
        {
            col[i].enabled = true;

            RaycastHit RCH;
            if (col[i].Raycast(ray, out RCH, Mathf.Infinity))
            {

                ray.origin = RCH.point;
                ray.direction = (RCH.point - transform.position).normalized;

                float rayDist = RCH.distance;

                if (col[i].Raycast(ray, out RCH, Mathf.Infinity))
                {
                    if (rayDist >= RCH.distance)
                    {
                        G.AddAtmosphereForce(P);

                        col[i].enabled = false;
                        return true;
                    }
                }

                col[i].enabled = false;
            }
            else
            {
                G.AddAtmosphereForce(P);

                col[i].enabled = false;
                return true;
            }
        }

        return false;
    }
}
