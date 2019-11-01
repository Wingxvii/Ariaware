using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebugObject : MonoBehaviour
{
    float FPSaverage = 0f;
    int samples = 0;

    private void Update()
    {
        FPSaverage = (FPSaverage * samples + Time.deltaTime) / (samples + 1);
        if (samples < 50)
            samples++;
        Debug.Log((int)(1f / FPSaverage) + ", " + FPSaverage);
    }
}

