using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModuleAxis : Module
{
    public float value
    {
        get { return GetAxis(); }
    }
    protected abstract float GetAxis();
}
