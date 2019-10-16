using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleButtonMouse : ModuleButton
{
    public MouseButton mouseButton = MouseButton.None;

    protected override bool GetPressed()
    {
        return GetButtonPressed();
    }

    bool GetButtonPressed()
    {
        switch (mouseButton)
        {
            case MouseButton.Left:
                return Input.GetMouseButton(0);
            case MouseButton.Right:
                return Input.GetMouseButton(1);
            case MouseButton.Middle:
                return Input.GetMouseButton(2);
            default:
                return false;
        }
    }

    public enum MouseButton
    {
        None,
        Left,
        Right,
        Middle
    }
}
