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

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            Cam = GetComponent<Camera>();
            LocalBody = new JoinedVar<CameraAnchor, Body>(this, false);

            return true;
        }

        return false;
    }

    protected override bool CrossBranchInitialize()
    {
        if (base.CrossBranchInitialize())
        {
            EntityContainer ec = Container.GetObj(0);
            if (ec.TreeInit())
            {
                for (int i = 0; i < ec.AttachedSlots.Amount; ++i)
                {
                    SlotBase sb = ec.AttachedSlots.GetObj(i);
                    if (sb.BranchInit() && FType.FindIfType(sb.GetSlotType(), typeof(Body)))
                    {
                        for (int j = 0; j < sb.EntityPlug.Amount; ++j)
                        {
                            Body b = EType<Body>.FindType(sb.EntityPlug.GetObj(j));
                            if (b != null && b.TreeInit())
                            {
                                LocalBody.Attach(b.LocalCamera);
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {
        LocalBody.Yeet();

        base.HierarchyDeInitialize();
    }

    protected override void DestroyVars()
    {
        LocalBody = null;
        Cam = null;

        base.DestroyVars();
    }
}
