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
    Vector3 midscreen;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            midscreen = new Vector3(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2, 0);

            AddUpdate();

            AddLateUpdate();

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

    protected override Vector3 GetDirectionalInput()
    {
        Vector3 output = Vector3.zero;

        output.x += mouseDirection(Xaxis);
        output.y += mouseDirection(Yaxis);
        output.z += mouseDirection(Zaxis);

        return output;
    }

    protected override void UpdateObject()
    {
        if (!toggleLock)
            deltaMouse = Input.mousePosition - mousePrevious;
        else
            deltaMouse = ToggleMouseLock.GetCursorInvY() - mousePrevious;
    }

    protected override void LateUpdateObject()
    {
        if (!toggleLock)
        {
            mousePrevious = Input.mousePosition;
        }
        else
        {
            //Vector3 midscreen = new Vector3(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2, 0);
            //Vector3 winCent = (ToggleMouseLock.GetCursorInvY() - Input.mousePosition + midscreen);
            //mousePrevious = Input.mousePosition;
            //Debug.Log(winCent + ", WINDOWS CENTER");
            //Debug.Log(midscreen + ", UNITY CENTER");
            //Debug.Log(ToggleMouseLock.GetCursorInvY() + ", WINDOWS CURSOR");
            //Debug.Log(Input.mousePosition + ", UNITY CURSOR");
            //mousePrevious = ToggleMouseLock.GetCursorInvY() - Input.mousePosition + midscreen;
            //Debug.Log(mousePrevious + ", <-------- PREVIOUS");
            mousePrevious = midscreen;
            //Debug.Log(mousePrevious + ", <----------- NEW!!!!!!!!!");
            ToggleMouseLock.SetCursorInvY(mousePrevious);

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
