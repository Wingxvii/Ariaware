using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Rigidbody))]
public class Body : Puppet
{
    public JoinedVar<Body, Spawnpoint> spawn;
    public Rigidbody Rb { get; protected set; }
    public JoinedVar<Body, CameraAnchor> LocalCamera;
    public Collider[] Col;
    public JoinedList<Body, Inventory> inventories;
    public bool notYourBody = true;

    //public float IDENT_VELOCITY = 0f;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddLateUpdate();

            Rb = GetComponent<Rigidbody>();
            LocalCamera = new JoinedVar<Body, CameraAnchor>(this);
            inventories = new JoinedList<Body, Inventory>(this);
            Col = GetComponentsInChildren<Collider>();
            spawn = new JoinedVar<Body, Spawnpoint>(this);

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

            AttachSpawnpoint();

            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {
        LocalCamera.Yeet();
        inventories.Yeet();
        spawn.Yeet();

        base.CrossBranchDeInitialize();
    }

    protected override void DestroyVars()
    {
        LocalCamera = null;
        Rb = null;
        Col = null;
        inventories = null;
        spawn = null;

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

    public void AttachSpawnpoint()
    {
        EntityContainer ec = Container.GetObj(0);
        if (ec.TreeInit())
        {
            for (int i = 0; i < ec.AttachedSlots.Amount; ++i)
            {
                SlotBase sb = ec.AttachedSlots.GetObj(i);
                if (FType.FindIfType(sb.GetSlotType(), typeof(Spawnpoint)) && sb.BranchInit())
                {
                    for (int j = 0; j < sb.EntityPlug.Amount; ++j)
                    {
                        Spawnpoint sp = EType<Spawnpoint>.FindType(sb.EntityPlug.GetObj(j));
                        if (sp != null && sp.TreeInit())
                        {
                            spawn.Attach(sp.spawnableBody);
                            return;
                        }
                    }
                    return;
                }
            }
        }
    }

    protected override void LateUpdateObject()
    {
        if (Container.GetObj(0) != null)
        {
            uint pState = Container.GetObj(0).pState;

            if ((pState & (uint)PlayerState.Alive) == 0)
            {
                pState = pState | (uint)PlayerState.Alive;

                if (spawn.GetObj(0) != null)
                {
                    transform.position = spawn.GetObj(0).transform.position;
                }
            }
        }
    }
}
