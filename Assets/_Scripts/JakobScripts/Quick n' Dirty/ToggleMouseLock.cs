using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
    public int X;
    public int Y;

    public POINT(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
}

public class ToggleMouseLock : MonoBehaviour
{
    [DllImport("User32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    [DllImport("User32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCursorPos(out POINT p);

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            ModuleDirectionalMouse.toggleLock = !ModuleDirectionalMouse.toggleLock;
            Cursor.visible = !Cursor.visible;
        }
    }

    public static Vector3 GetCursor()
    {
        POINT p;
        GetCursorPos(out p);

        return new Vector3(p.X, p.Y, 0);
    }

    public static Vector3 GetCursorInvY()
    {
        POINT p;
        GetCursorPos(out p);

        return new Vector3(p.X, Screen.currentResolution.height - p.Y, 0);
    }

    public static void SetCursor(Vector3 position)
    {
        SetCursorPos((int)position.x, (int)position.y);
    }

    public static void SetCursorInvY(Vector3 position)
    {
        SetCursorPos((int)position.x, Screen.currentResolution.height - (int)position.y);
    }
}
