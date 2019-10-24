using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleDirectionalMouse : ModuleDirectional
{
    public static bool toggleLock = false;
    public float scrollAmplifier = 1f;
    public float mouseSensitivity = 1f;

    public Axis Xaxis = Axis.MouseX;
    public Axis Yaxis = Axis.MouseY;
    public Axis Zaxis = Axis.None;

    public static Vector3 mousePrevious;
    Vector3 deltaMouse = Vector3.zero;

    protected override bool CrossBranchInitialize()
    {
        if (base.CrossBranchInitialize())
        {
            mousePrevious = Input.mousePosition;

            return true;
        }

        return false;
    }

    protected override Vector3 GetDirectionalInput()
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
    }

    private void LateUpdate()
    {
        if (!toggleLock)
        {
            mousePrevious = Input.mousePosition;
        }
        else
        {
            Vector3 midscreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            //Vector3 winCent = (ToggleMouseLock.GetCursorInvY() - Input.mousePosition + midscreen);
            //mousePrevious = Input.mousePosition;
            //Debug.Log(winCent + ", WINDOWS CENTER");
            //Debug.Log(midscreen + ", UNITY CENTER");
            //Debug.Log(ToggleMouseLock.GetCursorInvY() + ", WINDOWS CURSOR");
            //Debug.Log(Input.mousePosition + ", UNITY CURSOR");
            mousePrevious = ToggleMouseLock.GetCursorInvY() - Input.mousePosition + midscreen;
            ToggleMouseLock.SetCursorInvY(ToggleMouseLock.GetCursorInvY() - Input.mousePosition + midscreen);

            mousePrevious = midscreen;
        }
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
