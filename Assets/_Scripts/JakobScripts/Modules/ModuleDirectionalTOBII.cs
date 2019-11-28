using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleDirectionalTOBII : ModuleDirectional
{
    public float deadRadius = 0.2f;
    public float turnSpeed = 50f;
    public float turningPower = 1.5f;
    public float lerpPower = 1f;

    static public Vector2 LERPpos { get; private set; } = Vector2.zero;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddLateUpdate();

            //LERPpos = Tobii.Gaming.TobiiAPI.GetGazePoint().Screen;

            return true;
        }

        return false;
    }

    protected override Vector3 GetDirectionalInput()
    {
        if (Tobii.Gaming.TobiiAPI.IsConnected)
        {
            Vector2 newVec = LERPpos - new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            Vector2 vecNorm = newVec.normalized;

            newVec.x = Mathf.Clamp(newVec.x / Screen.width * 2f, -1f, 1f);
            newVec.y = Mathf.Clamp(newVec.y / Screen.height * 2f, -1f, 1f);

            float strength = Mathf.Pow(Mathf.Max(0, newVec.magnitude - deadRadius), turningPower) * turnSpeed;

            Vector2 returnVec = vecNorm * strength * Time.deltaTime;

            return new Vector2(returnVec.y, returnVec.x);
        }

        return new Vector2(0, 0);
    }

    protected override void LateUpdateObject()
    {
        if (Tobii.Gaming.TobiiAPI.IsConnected)
        {
            //Debug.Log(1f - Mathf.Pow(1f - lerpPower, Time.deltaTime));
            LERPpos = Vector2.Lerp(LERPpos, Tobii.Gaming.TobiiAPI.GetGazePoint().Screen, 1f - Mathf.Pow(1f - lerpPower, Time.deltaTime));
            //Debug.Log(LERPpos);
        }
    }
}
