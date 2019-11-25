using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class TOBIIkeeper
{
    public Quaternion tobiiPos { get; private set; }
    private TOBIIkeeper() { }
    Vector2 realPoint = Vector3.zero;
    Ray SaveRay;
    //Camera theCamera;

    private static TOBIIkeeper tk;
    public static TOBIIkeeper TK
    {
        get { if (tk == null) { tk = new TOBIIkeeper(); tk.SaveRay = new Ray(); } return tk; }
    }

    public void UPDATE_TOBII()
    {
        realPoint = TobiiAPI.GetGazePoint().Screen;
        SaveRay = Camera.main.ScreenPointToRay(realPoint);
        tobiiPos = Quaternion.FromToRotation(Camera.main.transform.forward, SaveRay.direction);
    }

    public void UPDATE_SCREEN()
    {
        //theCamera = Camera.main;
    }

    public void DEBUG_TOBII()
    {
        Debug.Log(Camera.main);
        Debug.Log(Screen.width + ", " + Screen.height);
        Debug.Log(SaveRay.direction);
    }
}
