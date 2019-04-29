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

    static Vector3 checkKey(Vector3 d, KeyCode k)
    {
        if (Input.GetKey(k))
        {
            return d;
        }
        return Vector3.zero;
    }
}
