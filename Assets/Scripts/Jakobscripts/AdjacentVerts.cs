using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjacentVerts
{
    public static float bias = 0.0f;
    List<int> itself;
    List<int> adjacent;
    bool ticked = false;

    public void SetVert(List<Vector4> sortedVerts, int LB, int RB)
    {
        if (itself == null)
        {
            itself = new List<int>();
            adjacent = new List<int>();
        }

        if (itself.Count > 0)
        {
            itself.Clear();
            adjacent.Clear();
        }

        for (int i = LB; i <= RB; i++)
        {
            itself.Add((int)sortedVerts[i].w);
        }

        //for (int i = 0; i < verts.Length; i++)
        //{
        //    if (1.0f - Vector3.Dot(verts[i], verts[index]) < bias)
        //    {
        //        if (index != i)
        //        {
        //            itself.Add(i);
        //        }
        //    }
        //}
    }

    public void AddAdjacency(int[] tIndices, int index)
    {
        int floor = index - index % 3;

        for (int i = floor; i < floor + 3; i++)
        {
            InsertAdjacent(tIndices[i]);
        }
    }

    void InsertAdjacent(int ADJ)
    {
        bool succ = true;

        for (int i = 0; i < itself.Count; i++)
        {
            succ = succ && (itself[i] == ADJ);
        }

        for (int i = 0; i < adjacent.Count; i++)
        {
            succ = succ && (adjacent[i] == ADJ);
        }

        if (succ)
        {
            adjacent.Add(ADJ);
        }
    }

    public void Tick()
    {
        ticked = true;
    }

    public void UnTick()
    {
        ticked = false;
    }

    public bool IsTicked()
    {
        return ticked;
    }

    public List<int> Copies()
    {
        return itself;
    }

    public List<int> Nearby()
    {
        return adjacent;
    }
}
