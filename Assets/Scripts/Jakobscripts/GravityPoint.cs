using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPoint : MonoBehaviour
{
    public pullType gEq = pullType.inverseSquared;

    public float pullScale = 1.00f;
    public float areaOfEffect { get; private set; }
    float bias = 0.001f;
    public float radialCutOff = -1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float pullMag(float r)
    {
        float R = r + bias;
        switch (gEq)
        {
            case pullType.inverse:
                return 1f / R;
            case pullType.inverseSquared:
                return 1f / (R * R);
            case pullType.inverseExponentialE:
                return Mathf.Exp(-R);
            case pullType.constant:
                return 1f;
        }
        return 0;
    }

    public float oppositeEq(float g)
    {
        switch (gEq)
        {
            case pullType.inverse:
                return 1f / g - bias;
            case pullType.inverseSquared:
                return 1f / Mathf.Sqrt(g) - bias;
            case pullType.inverseExponentialE:
                return -Mathf.Log(g) - bias;
            case pullType.constant:
                return float.MaxValue;
        }
        return 0;
    }

    public void setAOE(float AOE)
    {
        areaOfEffect = AOE;
    }

    public enum pullType
    {
        inverse,
        inverseSquared,
        inverseExponentialE,
        constant
    }
}
