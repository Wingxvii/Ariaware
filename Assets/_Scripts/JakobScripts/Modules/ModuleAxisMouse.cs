using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleAxisMouse : ModuleAxis
{
    public float scrollAmplifier = 1f;
    public float mouseSensitivity = 1f;

    public Axis mouseAxis = Axis.None;

    Vector3 mousePrevious;
    Vector3 deltaMouse = Vector3.zero;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddUpdate();

            return true;
        }

        return false;
    }

    protected override bool CrossBranchInitialize()
    {
        if (base.CrossBranchInitialize())
        {
            mousePrevious = Input.mousePosition;

            return true;
        }

        return false;
    }

    protected override float GetAxis()
    {
        return mouseDirection(mouseAxis);
    }

    protected override void UpdateObject()
    {
        deltaMouse = Input.mousePosition - mousePrevious;
        mousePrevious = Input.mousePosition;
    }

    float mouseDirection(Axis axis)
    {
        switch (axis)
        {
            case Axis.MouseX:
                return deltaMouse.x * mouseSensitivity;
            case Axis.MouseInvertX:
                return -deltaMouse.x * mouseSensitivity;
            case Axis.MouseY:
                return deltaMouse.y * mouseSensitivity;
            case Axis.MouseInvertY:
                return -deltaMouse.y * mouseSensitivity;
            case Axis.MouseScroll:
                return Input.mouseScrollDelta.y * scrollAmplifier;
            case Axis.MouseInvertScroll:
                return -Input.mouseScrollDelta.y * scrollAmplifier;
            default:
                return 0;
        }
    }

    public enum Axis
    {
        None,
        MouseX,
        MouseInvertX,
        MouseY,
        MouseInvertY,
        MouseScroll,
        MouseInvertScroll
    }
}
