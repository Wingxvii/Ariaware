using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]

public class TerraformTwo : MonoBehaviour
{
    Camera MC;
    Renderer MAT;
    RaycastHit RCH;
    MeshCollider col;
    Mesh MR;

    Vector3[] verticesNormalized;
    float[] vertexLengths;
    Vector3[] vertices;

    public float intensity = 0.01f;
    public float influence = 0.1f;
    public float minDist = 0.9f;
    public float maxDist = 1.1f;

    private void Awake()
    {
        col = GetComponent<MeshCollider>();
        MC = FindObjectOfType<Camera>();
        MAT = GetComponent<Renderer>();
        MR = GetComponent<MeshFilter>().mesh;

        int LEN = MR.vertices.Length;
        verticesNormalized = new Vector3[LEN];
        vertexLengths = new float[LEN];
        vertices = new Vector3[LEN];

        for (int i = 0; i < LEN; i++)
        {
            verticesNormalized[i] = MR.vertices[i].normalized;
            vertexLengths[i] = MR.vertices[i].magnitude;
            vertices[i] = verticesNormalized[i] * vertexLengths[i];


        }

        MR.vertices = vertices;

        col.sharedMesh = MR;
    }


}