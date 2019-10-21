using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ControlLocal : Modifier
{
    public bool controlPosition = false;
    public Vector3 snapPosition = Vector3.zero;

    public bool controlRotation = false;
    public Vector3 snapAngles = Vector3.zero;

    public bool applyOnUpdate = true;
    public bool applyOnFixedUpdate = false;
    public bool applyOnPreRender = false;
    public bool applyOnLateUpdate = false;

    public bool SetToInitial = false;
    Vector3 initialP = Vector3.zero;
    Quaternion initialR = Quaternion.identity;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            initialP = transform.localPosition;
            initialR = transform.localRotation;

            return true;
        }

        return false;
    }

    protected override void DestroyVars()
    {
        base.DestroyVars();
    }

    private void Update()
    {
        if (applyOnUpdate)
            ApplyUpdateModifier();
    }

    private void LateUpdate()
    {
        if (applyOnLateUpdate)
            ApplyUpdateModifier();
    }

    private void FixedUpdate()
    {
        if (applyOnFixedUpdate)
            ApplyUpdateModifier();
    }

    private void OnPreRender()
    {
        if (applyOnPreRender)
            ApplyUpdateModifier();
    }

    public void ApplyUpdateModifier()
    {
        if (SetToInitial)
        {
            transform.localPosition = initialP;
            transform.localRotation = initialR;
        }
        else
        {
            if (controlPosition)
                transform.localPosition = snapPosition;

            if (controlRotation)
                transform.localRotation = Quaternion.Euler(snapAngles);
        }
    }
}
