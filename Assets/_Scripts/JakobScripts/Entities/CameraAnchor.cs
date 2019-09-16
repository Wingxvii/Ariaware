using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAnchor : Puppet
{
    Camera cam;
    public Camera Cam
    {
        get { return cam; }
        protected set { cam = value; }
    }

    JoinedVar<CameraAnchor, Body> localBody;
    public JoinedVar<CameraAnchor, Body> LocalBody
    {
        get { return localBody; }
        set { localBody = value; }
    }

    protected override void Initialize()
    {
        base.Initialize();

        EntityContainer ec = Container.GetObj(0);
        if (ec != null)
        {
            BodySlot bs = ec.GetComponent<BodySlot>();
            if (bs != null && bs.isActiveAndEnabled)
            {
                Entity ent = bs.ObjectSlot.GetObj(0);
                if (ent != null && ent.isActiveAndEnabled)
                {
                    Body b = EType<Body>.FindType(ent);
                    LocalBody.Attach(b.LocalCamera);
                }
            }
        }
    }

    protected override void CreateVars()
    {
        base.CreateVars();

        Cam = GetComponent<Camera>();
        LocalBody = new JoinedVar<CameraAnchor, Body>(this, false);
    }

    protected override void DeInitialize()
    {
        LocalBody.Yeet();

        base.DeInitialize();
    }

    protected override void DestroyVars()
    {


        base.DestroyVars();
    }

    protected override SlotBase GetSlot()
    {
        return Container.GetObj(0).GetComponent<CameraAnchorSlot>();
    }
}
