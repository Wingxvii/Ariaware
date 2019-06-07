using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class PlanetContainer : MonoBehaviour
{
    Rigidbody rb;
    Planet child;
    // Start is called before the first frame update
    void Awake()
    {
        child = GetComponentInChildren<Planet>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.angularDrag = 0;
        rb.drag = 0;
        rb.mass = 1;
    }

    public Planet GetPlanet()
    {
        return child;
    }
}
