using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    static public Vector3 receivePlayerKeyboard()
    {
        Vector3 pRet = Vector3.zero;

        pRet += checkKey(Vector3.forward, KeyCode.W);
        pRet += checkKey(Vector3.left, KeyCode.A);
        pRet += checkKey(Vector3.back, KeyCode.S);
        pRet += checkKey(Vector3.right, KeyCode.D);

        if (pRet.magnitude > 0)
            return pRet.normalized;
        
        return pRet;
    }

    static public Vector3 receivePlayerJump(bool isGrounded)
    {
        Vector3 pRet = Vector3.zero;
        if (isGrounded)
            pRet += checkKey(Vector3.up, KeyCode.Space);

        return pRet;
    }

    static public bool hasPlayerJumped()
    {
        return Input.GetKey(KeyCode.Space);
    }

    static public Vector3 receivePlayerKeyboardArrows()
    {
        Vector3 pRet = Vector3.zero;

        pRet += checkKey(Vector3.forward, KeyCode.UpArrow);
        pRet += checkKey(Vector3.left, KeyCode.LeftArrow);
        pRet += checkKey(Vector3.back, KeyCode.DownArrow);
        pRet += checkKey(Vector3.right, KeyCode.RightArrow);

        if (pRet.magnitude > 0)
            return pRet.normalized;

        return pRet;
    }

    static public Vector3 receiveCameraKeyboard()
    {
        Vector3 pRet = Vector3.zero;

        pRet += checkKey(Vector3.forward, KeyCode.W);
        pRet += checkKey(Vector3.left, KeyCode.A);
        pRet += checkKey(Vector3.back, KeyCode.S);
        pRet += checkKey(Vector3.right, KeyCode.D);
        pRet += checkKey(Vector3.up, KeyCode.Q);
        pRet += checkKey(Vector3.down, KeyCode.E);

        if (pRet.magnitude > 0)
            return pRet.normalized;

        return pRet;
    }

    static public Vector2 mouseRot()
    {
        Vector2 pRet = Vector2.zero;

        pRet.x += Input.GetAxis("Mouse X");
        pRet.y += Input.GetAxis("Mouse Y");

        return pRet;
    }

    static public Vector2 mousePos()
    {
        return Input.mousePosition;
    }

    static public Vector2 mousePosNormalized()
    {
        Vector2 v2 = Vector2.zero;

        v2.x /= Screen.width;
        v2.y /= Screen.height;

        return v2;
    }

    static Vector2 screenSize()
    {
        return new Vector2(Screen.width, Screen.height);
    }

    static public Ray mouseRay(Camera c)
    {
        Ray returnRay = new Ray();

        returnRay.origin = c.transform.position;
        float depth = screenDepth(c.fieldOfView, Screen.height);
        Vector2 screen = screenSize();

        Vector2 mPos = mousePos() - screen * 0.5f;
        Vector3 outwards = c.transform.rotation * new Vector3(mPos.x, mPos.y, depth);

        returnRay.direction = outwards.normalized;

        return returnRay;
    }

    static float screenDepth(float fovY, float screenHeight)
    {
        return 0.5f * screenHeight / Mathf.Tan(fovY * Mathf.Deg2Rad * 0.5f);
    }

    static public float mouseScroll()
    {
        return Input.mouseScrollDelta.y;
    }

    static Vector3 checkKey(Vector3 d, KeyCode k)
    {
        if (Input.GetKey(k))
        {
            return d;
        }
        return Vector3.zero;
    }
}
