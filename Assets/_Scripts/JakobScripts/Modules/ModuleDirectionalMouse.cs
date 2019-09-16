using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleDirectionalMouse : ModuleDirectional
{
    public float scrollAmplifier = 1f;
    public float mouseSensitivity = 1f;

    public Axis Xaxis = Axis.MouseX;
    public Axis Yaxis = Axis.MouseY;
    public Axis Zaxis = Axis.None;

    Vector3 mousePrevious;
    Vector3 deltaMouse = Vector3.zero;

    protected override void Initialize()
    {
        base.Initialize();

        mousePrevious = Input.mousePosition;
    }

    protected override void CreateVars()
    {
        base.CreateVars();
    }

    protected override void DeInitialize()
    {
        base.DeInitialize();
    }

    protected override void DestroyVars()
    {
        base.DestroyVars();
    }

    public override Vector3 GetDirectionalInput()
    {
        Vector3 output = Vector3.zero;

        output.x += mouseDirection(Xaxis);
        output.y += mouseDirection(Yaxis);
        output.z += mouseDirection(Zaxis);

        return output;
    }

    private void Update()
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
