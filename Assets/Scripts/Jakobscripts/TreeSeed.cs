using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TreeSeed : MonoBehaviour
{
    public TreeSettings TS;
    public int numberOfConnections = 0;

    List<TreeNode> allNodes;

    void Awake()
    {
        beginGeneration();
    }

    void beginGeneration()
    {
        //Debug.Log(Quaternion.Euler(45f, 0, 0) * Vector3.up);

        allNodes = new List<TreeNode>();
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        if (TS != null)
        {
            if (TS.nodeType != null && TS.connection != null)
            {
                TreeNode TN = Instantiate(TS.nodeType);
                TN.recursiveGeneration(allNodes, TS, TS.initialScale, transform,
                    Vector3.up, 0);
            }
        }

        numberOfConnections = allNodes.Count;
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {

    }
}
