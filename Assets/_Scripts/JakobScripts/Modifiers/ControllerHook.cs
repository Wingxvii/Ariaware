using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace UnityEngine
{
    //public struct Vec3
    //{
    //    public float x;
    //    public float y;
    //    public float z;
    //}
}

[RequireComponent(typeof(Controller))]
public class ControllerHook : Modifier
{
    Controller ctrol;
    Body bod;

    [DllImport("CNET.dll")]
    static extern void SendMsg(string str, IntPtr client);          //Sends Message to all other clients    
    [DllImport("CNET.dll")]
    internal static extern void SendTransformation(Vec3 pos, Vec3 rot, IntPtr client);          //Sends Position data to all other clients
    [DllImport("CNET.dll")]
    static extern int GetPlayerNumber(IntPtr client);

    protected override void Initialize()
    {
        base.Initialize();

        if (ConnectionLoader.ip != null)
        {
            SetControllerSnapToPlayer(GetPlayerNumber(NetworkManager.NM.Client));
            Debug.Log(GetPlayerNumber(NetworkManager.NM.Client));
        }
    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();


    }

    protected override void CreateVars()
    {
        base.CreateVars();

        ctrol = GetComponent<Controller>();
    }

    protected override void DeInitialize()
    {


        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {


        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        ctrol = null;

        base.DestroyVars();
    }

    public void SetControllerSnapToPlayer(int num)
    {
        StopCoroutine(SendData());
        if (ConnectionLoader.ip != null)
        {
            for (int i = CentralManager.CM.SceneBodies.Count - 1; i >= 0; --i)
            {
                if (CentralManager.CM.SceneBodies[i].PlayerNum == num)
                {
                    transform.SetParent(CentralManager.CM.SceneBodies[i].transform);
                    if (bod != null)
                        StartCoroutine(SendData());
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (ConnectionLoader.ip != null)
        {
            if (bod != null)
            {
                if (NetworkManager._transformQueue.Count != 0)
                {
                    lock (NetworkManager._transformQueue)
                    {
                        foreach (List<string> data in NetworkManager._transformQueue)
                        {
                            this.ProcessTransform(data);
                        }
                        NetworkManager._transformQueue.Clear();
                    }
                }
            }
        }
    }

    IEnumerator SendData()
    {
        if (ConnectionLoader.ip != null)
        {
            while (true)
            {
                if (bod != null)
                {
                    Transform t = bod.transform;
                    Vector3 p = t.localPosition;
                    Vector3 r = t.localRotation.eulerAngles;
                    Vector4 s = t.localScale;

                    Vec3 pos, rot;
                    pos.x = p.x;
                    pos.y = p.y;
                    pos.z = p.z;
                    rot.x = r.x;
                    rot.y = r.y;
                    rot.z = r.z;
                    
                    SendTransformation(pos, rot, NetworkManager.NM.Client);

                    //if (NetworkManager._transformQueue.Count != 0)
                    //{
                    //    lock (NetworkManager._transformQueue)
                    //    {
                    //        foreach (List<string> data in NetworkManager._transformQueue)
                    //        {
                    //            this.ProcessTransform(data);
                    //        }
                    //        NetworkManager._transformQueue.Clear();
                    //    }
                    //}
                }

                yield return new WaitForFixedUpdate();
            }
        }
    }

    Vector3 ParseVec3(string x, string y, string z)
    {
        float X = float.Parse(x);
        float Y = float.Parse(y);
        float Z = float.Parse(z);
        return new Vector3(X, Y, Z);
    }

    private void ProcessTransform(List<string> parsedData)
    {
        int sender = int.Parse(parsedData[0]);

        for (int i = CentralManager.CM.SceneBodies.Count - 1; i >= 0; --i)
        {
            if (sender == CentralManager.CM.SceneBodies[i].PlayerNum)
            {
                CentralManager.CM.SceneBodies[i].transform.localPosition = ParseVec3(parsedData[1], parsedData[2], parsedData[3]);
                CentralManager.CM.SceneBodies[i].transform.localRotation = Quaternion.Euler(ParseVec3(parsedData[4], parsedData[5], parsedData[6]));
                //CentralManager.CM.SceneBodies[i].transform.localScale = ParseVec3(parsedData[7], parsedData[8], parsedData[9]);
            }
        }
    }

    private void OnTransformParentChanged()
    {
        EntityContainer ec = ctrol.Container.GetObj(0);
        if (ec != null && ec.AE)
        {
            BodySlot bs = ec.GetComponent<BodySlot>();
            if (bs != null && bs.AE)
            {
                Entity ent = bs.ObjectSlot.GetObj(0);
                if (ent != null && ent.AE)
                {
                    Body b = EType<Body>.FindType(ent);
                    if (b != null)
                    {
                        bod = b;
                    }
                }
            }
        }
    }
}
