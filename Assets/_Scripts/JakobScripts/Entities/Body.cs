using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Body : Puppet
{
    public Rigidbody Rb { get; protected set; }
    public JoinedVar<Body, CameraAnchor> LocalCamera;
    public Collider[] Col;
    public JoinedList<Body, Inventory> inventories;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            Rb = GetComponent<Rigidbody>();
            LocalCamera = new JoinedVar<Body, CameraAnchor>(this);
            inventories = new JoinedList<Body, Inventory>(this);
            Col = GetComponentsInChildren<Collider>();

            return true;
        }

        return false;
    }

    protected override bool CrossBranchInitialize()
    {
        if (base.CrossBranchInitialize())
        {
            AttachCamera();

            AttachInventories();

            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {
        LocalCamera.Yeet();
        inventories.Yeet();

        base.CrossBranchDeInitialize();
    }

    protected override void DestroyVars()
    {
        LocalCamera = null;
        Rb = null;
        Col = null;
        inventories = null;

        base.DestroyVars();
    }

    public void EnableColliders(bool enab)
    {
        for (int i = 0; i < Col.Length; ++i)
        {
            Col[i].gameObject.layer = enab ? 0 : 2;
        }
    }

    public void Damage(float dam)
    {
        for (int i = 0; i < JoinedStats.Amount; ++i)
        {
            HealthBar hb = EType<HealthBar>.FindType(JoinedStats.GetObj(i));
            if (hb != null)
            {
                hb.Damage(dam);
            }
        }
    }

    public void AttachCamera()
    {
        EntityContainer ec = Container.GetObj(0);
        if (ec.TreeInit())
        {
            for (int i = 0; i < ec.AttachedSlots.Amount; ++i)
            {
                SlotBase sb = ec.AttachedSlots.GetObj(i);
                if (FType.FindIfType(sb.GetSlotType(), typeof(CameraAnchor)) && sb.BranchInit())
                {
                    for (int j = 0; j < sb.EntityPlug.Amount; ++j)
                    {
                        CameraAnchor ca = EType<CameraAnchor>.FindType(sb.EntityPlug.GetObj(j));
                        if (ca != null && ca.TreeInit())
                        {
                            LocalCamera.Attach(ca.LocalBody);
                            return;
                        }
                    }
                    return;
                }
            }
        }
    }

    public void AttachInventories()
    {
        EntityContainer ec = Container.GetObj(0);
        if (ec.TreeInit())
        {
            for (int i = 0; i < ec.AttachedSlots.Amount; ++i)
            {
                SlotBase sb = ec.AttachedSlots.GetObj(i);
                if (FType.FindIfType(sb.GetSlotType(), typeof(Inventory)) && sb.BranchInit())
                {
                    for (int j = 0; j < sb.EntityPlug.Amount; ++j)
                    {
                        Inventory inv = EType<Inventory>.FindType(sb.EntityPlug.GetObj(j));
                        if (inv != null && inv.TreeInit())
                        {
                            inventories.Attach(inv.body);
                            return;
                        }
                    }
                    return;
                }
            }
        }
    }
}
