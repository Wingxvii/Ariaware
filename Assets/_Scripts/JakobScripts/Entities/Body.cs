using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

[RequireComponent(typeof(Rigidbody))]
public class Body : Puppet
{
    public int PlayerNum = 0;

    Rigidbody rb;
    public Rigidbody Rb
    {
        get { return rb; }
        private set { rb = value; }
    }

    List<Collider> col;
    public List<Collider> Col
    {
        get { if (col == null) { col = new List<Collider>(); } return col; }
        private set { col = value; }
    }

    JoinedVar<Body, CameraAnchor> localCamera;
    public JoinedVar<Body, CameraAnchor> LocalCamera
    {
        get { return localCamera; }
        set { localCamera = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();

        EntityContainer ec = Container.GetObj(0);
        if (ec != null)
        {
            CameraAnchorSlot cas = ec.GetComponent<CameraAnchorSlot>();
            if (cas != null && cas.AE)
            {
                Entity ent = cas.ObjectSlot.GetObj(0);
                if (ent != null && ent.AE)
                {
                    CameraAnchor ca = EType<CameraAnchor>.FindType(ent);
                    LocalCamera.Attach(ca.LocalBody);
                }
            }
        }
    }

    protected override void InnerInitialize()
    {
        base.InnerInitialize();

        CentralManager.CM.ProtectedAddBody(this);
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        LocalCamera = new JoinedVar<Body, CameraAnchor>(this, false);

        Collider[] c = GetComponents<Collider>();
        for (int i = c.Length - 1; i >= 0; --i)
            Col.Add(c[i]);
        Rb = GetComponent<Rigidbody>();

        Rb.freezeRotation = true;
    }

    protected override void DeInitialize()
    {
        LocalCamera.Yeet();

        base.DeInitialize();
    }

    protected override void DeInnerInitialize()
    {
        CentralManager.CM.RemoveBody(this);

        base.DeInnerInitialize();
    }

    protected override void DestroyVars()
    {
        Rb = null;
        Col = null;

        base.DestroyVars();
    }

    protected override SlotBase GetSlot()
    {
        return Container.GetObj(0).GetComponent<BodySlot>();
    }
}
