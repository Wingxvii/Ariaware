using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    Collider col;
    //float thickness = 0.001f;
    //float control = 1.00f;

    void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false;
    }

    public bool moves(Gravitized G, Ray ray, Planet P)
    {
        ray.origin = G.transform.position;
        ray.direction = (transform.position - G.transform.position).normalized;

        col.enabled = true;

        RaycastHit RCH;
        if (col.Raycast(ray, out RCH, Mathf.Infinity))
        {

            ray.origin = RCH.point;
            ray.direction = (RCH.point - transform.position).normalized;

            float rayDist = RCH.distance;

            if (col.Raycast(ray, out RCH, Mathf.Infinity))
            {
                if (rayDist >= RCH.distance)
                {
                    G.suggestParent(P.transform);

                    col.enabled = false;
                    return true;
                }
            }

            col.enabled = false;
            return false;
        }

        G.suggestParent(P.transform);

        col.enabled = false;
        return true;
    }
}
