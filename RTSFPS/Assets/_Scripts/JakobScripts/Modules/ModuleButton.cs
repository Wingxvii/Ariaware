using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModuleButton : Module
{
    public bool pressed
    {
        get { return GetPressed(); }
    }
    protected abstract bool GetPressed();
}
