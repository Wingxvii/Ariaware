using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildUnit
{
    public BuildUnit(uint _ID, uint _type, Vector3 _position)
    {
        ID = _ID;
        type = _type;
        position = _position;
    }

    public uint ID;
    public uint type;
    public Vector3 position;
}

[RequireComponent(typeof(RTSFactory))]
public class ReadBuild : ReadBase
{
    RTSFactory RTSF;

    public Queue<BuildUnit> Build { get; protected set; }

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddFirst();

            if (FPSManager.FM.NDM.Init())
                NET_PACKET.NetworkDataManager.builds.Add(this);

            RTSF = GetComponent<RTSFactory>();

            Build = new Queue<BuildUnit>();

            return true;
        }

        return false;
    }

    protected override void DestroyVars()
    {
        RTSF = null;

        base.DestroyVars();
    }

    protected override void First()
    {
        if (NDM != null && NDM.Init())
        {
            NDM.ChugBuildQueue();
        }

        while (Build.Count > 0)
        {
            RTSF.SpawnObject((EntityType)Build.Peek().type, Build.Peek().position);
            Build.Dequeue();
        }
    }
}
