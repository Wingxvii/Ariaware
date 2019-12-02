using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : Entity
{
    public JoinedVar<Spawnpoint, Body> spawnableBody;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            spawnableBody = new JoinedVar<Spawnpoint, Body>(this);

            return true;
        }

        return false;
    }

    protected override bool CrossBranchInitialize()
    {
        if (base.CrossBranchInitialize())
        {
            AttachBody();

            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {
        spawnableBody.Yeet();

        base.CrossBranchDeInitialize();
    }

    protected override void DestroyVars()
    {
        spawnableBody = null;

        base.DestroyVars();
    }

    public void AttachBody()
    {
        EntityContainer ec = Container.GetObj(0);
        if (ec.TreeInit())
        {
            for (int i = 0; i < ec.AttachedSlots.Amount; ++i)
            {
                SlotBase sb = ec.AttachedSlots.GetObj(i);
                if (FType.FindIfType(sb.GetSlotType(), typeof(Body)) && sb.BranchInit())
                {
                    for (int j = 0; j < sb.EntityPlug.Amount; ++j)
                    {
                        Body b = EType<Body>.FindType(sb.EntityPlug.GetObj(j));
                        if (b != null && b.TreeInit())
                        {
                            spawnableBody.Attach(b.spawn);
                            transform.position = b.transform.position;
                            return;
                        }
                    }
                    return;
                }
            }
        }
    }
}
