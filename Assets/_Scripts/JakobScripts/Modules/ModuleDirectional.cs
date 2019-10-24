using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModuleDirectional : Module
{
    public Vector3 direction
    {
        get { return GetDirectionalInput(); }
    }
    protected abstract Vector3 GetDirectionalInput();
}
