using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Body : Puppet
{
    public Rigidbody Rb { get; protected set; }
    public JoinedVar<Body, CameraAnchor> LocalCamera;
    public Collider[] Col;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            Rb = GetComponent<Rigidbody>();
            LocalCamera = new JoinedVar<Body, CameraAnchor>(this);
            Col = GetComponents<Collider>();

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
                    if (sb.BranchInit() && FType.FindIfType(sb.GetSlotType(), typeof(CameraAnchor)))
                    {
                        for (int j = 0; j < sb.EntityPlug.Amount; ++j)
                        {
                            CameraAnchor ca = EType<CameraAnchor>.FindType(sb.EntityPlug.GetObj(j));
                            if (ca != null && ca.TreeInit())
                            {
                                LocalCamera.Attach(ca.LocalBody);
                            }
                        }
                    }
                }
            }

            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {
        LocalCamera.Yeet();

        base.CrossBranchDeInitialize();
    }

    protected override void DestroyVars()
    {
        LocalCamera = null;
        Rb = null;
        Col = null;

        base.DestroyVars();
    }

    public void Damage(float dam)
    {

    }
}
