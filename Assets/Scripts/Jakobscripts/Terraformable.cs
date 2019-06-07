using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
public class Terraformable : MonoBehaviour
{
    RaycastHit RCH;
    MeshCollider col;
    Camera MC;
    Renderer MAT;
    List<Vector3> hitP;
    Mesh MR;
    Vector3[] verticesNormalized;
    float[] vertexLengths;
    Vector3[] vertices;
    AdjacentVerts[] adjacent;
    List<int> clearList;
    List<Vector4> pointAndIndex;

    bool isHitting = false;
    public float intensity = 0.01f;
    public float influence = 0.1f;
    public float minDist = 0.9f;
    public float maxDist = 1.1f;

    private void Awake()
    {
        pointAndIndex = new List<Vector4>();
        clearList = new List<int>();
        hitP = new List<Vector3>();
        col = GetComponent<MeshCollider>();
        MC = FindObjectOfType<Camera>();
        MAT = GetComponent<Renderer>();
        MR = GetComponent<MeshFilter>().mesh;

        int LEN = MR.vertices.Length;
        verticesNormalized = new Vector3[LEN];
        vertexLengths = new float[LEN];
        vertices = new Vector3[LEN];
        adjacent = new AdjacentVerts[LEN];

        //AdjacentVerts ADJ;
        for (int i = 0; i < LEN; i++)
        {
            verticesNormalized[i] = MR.vertices[i].normalized;
            vertexLengths[i] = MR.vertices[i].magnitude;
            vertices[i] = verticesNormalized[i] * vertexLengths[i];
            pointAndIndex.Add(new Vector4(vertices[i].x, vertices[i].y, vertices[i].z, i));

            //ADJ = new AdjacentVerts();
            //ADJ.SetVert(MR.vertices, MR.triangles, i);
            //adjacent[i] = ADJ;
        }

        pointAndIndex.Sort(CompareVec4);

        //for (int i = 0; i < pointAndIndex.Count; i++)
        //{
        //    Debug.Log(pointAndIndex[i]);
        //}

        DoppleCheck();

        //for (int i = 0; i < pointAndIndex.Count; i++)
        //{
        //    //if (adjacent[i].Copies().Count > 1)
        //    //{
        //        Debug.Log(pointAndIndex[i]);
        //        for (int j = 0; j < adjacent[i].Copies().Count; j++)
        //        {
        //            Vector3 vV = vertices[adjacent[i].Copies()[j]];
        //            Debug.Log(new Vector4(vV.x, vV.y, vV.z, -i));
        //        }
        //    //}
        //}


        for (int i = 0; i < MR.triangles.Length; i++)
        {
            adjacent[MR.triangles[i]].AddAdjacency(MR.triangles, i);
        }

        MR.vertices = vertices;

        col.sharedMesh = MR;
    }

    private static int CompareVec4(Vector4 V1, Vector4 V2)
    {
        if (V1.x > V2.x)
        {
            return -1;
        }
        else if (V1.x < V2.x)
        {
            return 1;
        }

        if (V1.y > V2.y)
        {
            return -1;
        }
        else if (V1.y < V2.y)
        {
            return 1;
        }

        if (V1.z > V2.z)
        {
            return -1;
        }

        return 1;
    }

    private void DoppleCheck()
    {
        AdjacentVerts ADJ;

        for (int i = 0; i < pointAndIndex.Count; i++)
        {
            int CLAR = CheckLeftAndRight(i);

            for (int j = CLAR + i - 1; j >= i; j--)
            {
                ADJ = new AdjacentVerts();

                ADJ.SetVert(pointAndIndex, i, i + CLAR - 1);

                adjacent[(int)pointAndIndex[j].w] = ADJ;
            }

            i += (CLAR - 1);
        }
    }

    private int CheckLeftAndRight(int index)
    {
        int span = 1;

        while (span + index < pointAndIndex.Count && 
            pointAndIndex[index].x == pointAndIndex[index + span].x &&
            pointAndIndex[index].y == pointAndIndex[index + span].y &&
            pointAndIndex[index].z == pointAndIndex[index + span].z)
        {
            span++;
        }

        return span;
    }

    private void Update()
    {
        if (hitP.Count > 0)
        {
            hitP.Clear();
        }

        if (col.Raycast(Movement.mouseRay(MC), out RCH, float.PositiveInfinity))
        {
            isHitting = true;

            ChangeVerts(RCH.triangleIndex);

            SetVerts(RCH.triangleIndex);
        }
        else
        {
            isHitting = false;
        }

        for (int i = 0; i < clearList.Count; i++)
        {
            adjacent[clearList[i]].UnTick();
        }
        if (clearList.Count > 0)
        {
            clearList.Clear();
        }
    }

    void SetVerts(int index)
    {
        int vPoint;

        for (int i = 0; i < 3; i++)
        {
            vPoint = MR.triangles[index * 3 + i];
            hitP.Add(transform.TransformPoint(MR.vertices[vPoint]));
        }
    }

    void ChangeVerts(int index)
    {
        float mS = -Movement.mouseScroll();
        if (mS != 0)
        {
            int vPoint;

            for (int i = 0; i < 3; i++)
            {
                vPoint = MR.triangles[index * 3 + i];
                StrictEditVert(vPoint, mS);
            }

            MR.vertices = vertices;

            MR.RecalculateBounds();
            MR.RecalculateNormals();
            MR.RecalculateTangents();

            col.sharedMesh = MR;
        }
    }

    void StrictEditVert(int index, float mScroll)
    {
        List<int> copies = GetCopies(index);

        for (int i = 0; i < copies.Count; i++)
        {
            vertexLengths[copies[i]] += mScroll * intensity;
            if (vertexLengths[copies[i]] < minDist)
            {
                vertexLengths[copies[i]] = minDist;
            }
            else if (vertexLengths[copies[i]] > maxDist)
            {
                vertexLengths[copies[i]] = maxDist;
            }
            vertices[copies[i]] = vertexLengths[copies[i]] * verticesNormalized[copies[i]];
        }
    }

    List<int> GetCopies(int index)
    {
        List<int> retList = new List<int>();

        for (int i = 0; i < adjacent[index].Copies().Count; i++)
        {
            int c = adjacent[index].Copies()[i];
            retList.Add(c);
        }

        return retList;
    }

    private void OnDrawGizmos()
    {
        if (isHitting && MC != null)
        {
            Gizmos.color = Color.black;
            for (int i = 0; i < 3; i++)
                Gizmos.DrawSphere(hitP[i], 0.03f * 
                    Vector3.Dot(MC.transform.position - hitP[i], MC.transform.forward));
        }
    }
}
