using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSFactory : InitializableObject
{
    public List<EntityContainer> prefabList;

    Queue<EntityContainer>[] InactiveContainers;

    public List<EntityContainer> allContainers;
    List<EntityContainer>[] allContainerTypes;
    //int listIter = 0;
    public int BatchAmount = 1;

    public bool proper = true;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            allContainers = new List<EntityContainer>();
            InactiveContainers = new Queue<EntityContainer>[(int)EntityType.TOTAL];
            allContainerTypes = new List<EntityContainer>[(int)EntityType.TOTAL];

            for (int i = 0; i < InactiveContainers.Length; ++i)
            {
                InactiveContainers[i] = new Queue<EntityContainer>();
                allContainerTypes[i] = new List<EntityContainer>();
            }

            for (int i = 0; i < 3; ++i)
            {
                allContainers.Add(null);
            }

            return true;
        }

        return false;
    }

    public EntityContainer SpawnObject(EntityType et, int requestedID, Vector3 position)
    {
        //Debug.Log(allContainers.Count);
        int nextID = -1;
        if (!proper)
            nextID = FetchInactiveContainer(et);
        else
        {
            nextID = requestedID;
            MakeObject(et, nextID);
        }

        //Debug.Log(nextID);
        EntityContainer reference = allContainers[nextID];
        reference.gameObject.SetActive(true);
        reference.transform.SetParent(null);
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

    void MakeObject(EntityType et, int forceID)
    {
        while (allContainers.Count < forceID - 1)
        {
            allContainers.Add(null);
        }

        EnforceGeneration(et, forceID);
    }

    void EnforceGeneration(EntityType et, int forceID)
    {
        if (allContainers[forceID] == null)
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

                reference.ID = forceID + 1;
                reference.et = et;
                reference.transform.SetParent(transform);

                allContainers[forceID] = reference;
            }
        }
    }

    int FetchInactiveContainer(EntityType et)
    {
        if (InactiveContainers[(int)et].Count == 0)
        {
            GenerateNewContainers(et);
        }

        return InactiveContainers[(int)et].Dequeue().ID - 1;
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

            reference.ID = allContainers.Count + 1;
            reference.et = et;
            reference.transform.SetParent(transform);
            //Debug.Log(reference.ID);

            allContainers.Add(reference);
            InactiveContainers[(int)et].Enqueue(reference);
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

        allContainers[id - 1].gameObject.SetActive(false);
        allContainers[id - 1].transform.SetParent(transform);
        if (!proper)
            InactiveContainers[(int)allContainers[id - 1].et].Enqueue(allContainers[id - 1]);
    }
}
