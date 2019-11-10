using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSFactory : InitializableObject
{
    public List<EntityContainer> prefabList;

    Queue<EntityContainer>[] InactiveContainers;

    List<EntityContainer> allContainers;
    //int listIter = 0;
    public int BatchAmount = 1;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            allContainers = new List<EntityContainer>();
            InactiveContainers = new Queue<EntityContainer>[(int)EntityType.TOTAL];

            for (int i = 0; i < InactiveContainers.Length; ++i)
            {
                InactiveContainers[i] = new Queue<EntityContainer>();
            }

            return true;
        }

        return false;
    }

    public EntityContainer SpawnObject(EntityType et, Vector3 position)
    {
        int nextID = FetchInactiveContainer(et);
        EntityContainer reference = allContainers[nextID];
        reference.gameObject.SetActive(true);
        reference.transform.position = position;
        reference.transform.rotation = Quaternion.identity;

        return reference;

        //EntityContainer reference;
        //int inst = -1;
        //switch (et)
        //{
        //    case EntityType.Barracks:
        //        inst = 0;
        //        break;
        //    case EntityType.Droid:
        //        inst = 1;
        //        break;
        //    case EntityType.Turret:
        //        inst = 2;
        //        break;
        //    case EntityType.Wall:
        //        inst = 3;
        //        break;
        //    default:
        //        break;
        //}
        //if (inst >= 0)
        //{
        //    reference = Instantiate(prefabList[inst]);
        //    BasePACES[] bp = reference.GetComponentsInChildren<BasePACES>(true);
        //    for (int i = 0; i < bp.Length; ++i)
        //    {
        //        bp[i].ID = (int)type;
        //    }
        //    reference.transform.position = position;
        //
        //    allContainers.Add(reference);
        //
        //    return reference;
        //}

        //return null;
    }

    int FetchInactiveContainer(EntityType et)
    {
        if (InactiveContainers[(int)et].Count == 0)
        {
            GenerateNewContainers(et);
        }

        return InactiveContainers[(int)et].Dequeue().ID;
    }

    void GenerateNewContainers(EntityType et)
    {
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

        for (int i = 0; i < BatchAmount; ++i)
        {
            EntityContainer reference = Instantiate(prefabList[inst]);

            reference.ID = allContainers.Count;
            reference.et = et;
            reference.transform.SetParent(transform);

            allContainers.Add(reference);
            InactiveContainers[(int)et].Enqueue(Instantiate(prefabList[inst]));
        }
    }

    //public void NukeContainer(EntityContainer ec)
    //{
    //    allContainers.(ec);
    //    ec.gameObject.SetActive(false);
    //    Destroy(ec.gameObject);
    //}

    public void NukeContainer(int id)
    {
        //for (int i = allContainers.Count - 1; i >= 0; --i)
        //{
        //    if (allContainers[i].ID == id)
        //    {
        //        allContainers[i].gameObject.SetActive(false);
        //        Destroy(allContainers[i]);
        //        allContainers.RemoveAt(i);
        //        break;
        //    }
        //}

        allContainers[id].gameObject.SetActive(false);
        allContainers[id].transform.SetParent(transform);
        InactiveContainers[(int)allContainers[id].et].Enqueue(allContainers[id]);
    }
}
