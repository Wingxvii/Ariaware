using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSFactory : InitializableObject
{
    public List<EntityContainer> prefabList;

    List<EntityContainer> allContainers;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            allContainers = new List<EntityContainer>();

            return true;
        }

        return false;
    }

    public EntityContainer SpawnObject(EntityType et, uint type, Vector3 position)
    {
        EntityContainer reference;
        int inst = -1;
        switch (et)
        {
            case EntityType.Barracks:
                inst = 0;
                break;
            case EntityType.Droid:
                inst = 1;
                break;
            case EntityType.Turret:
                inst = 2;
                break;
            case EntityType.Wall:
                inst = 3;
                break;
            default:
                break;
        }
        if (inst >= 0)
        {
            reference = Instantiate(prefabList[inst]);
            BasePACES[] bp = reference.GetComponentsInChildren<BasePACES>(true);
            for (int i = 0; i < bp.Length; ++i)
            {
                bp[i].ID = (int)type;
            }
            reference.transform.position = position;

            allContainers.Add(reference);

            return reference;
        }

        return null;
    }

    public void NukeContainer(EntityContainer ec)
    {
        allContainers.Remove(ec);
        ec.gameObject.SetActive(false);
        Destroy(ec.gameObject);
    }

    public void NukeContainer(int id)
    {
        for (int i = allContainers.Count - 1; i >= 0; --i)
        {
            if (allContainers[i].ID == id)
            {
                allContainers[i].gameObject.SetActive(false);
                Destroy(allContainers[i]);
                allContainers.RemoveAt(i);
                break;
            }
        }
    }
}
