using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TreeNode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void recursiveGeneration(List<TreeNode> AN, TreeSettings TS, float tScale, 
        Transform parent, Vector3 vUp, int depth)
    {

        AN.Add(this);
        Collider col = GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }

        TreeNode connection = Instantiate(TS.connection);
        AN.Add(connection);

        col = connection.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        float bScale = tScale * (1f - TS.decay);

        float segLength = Random.Range(TS.segmentLengthLowerBound, TS.segmentLengthUpperBound) * bScale;

        Vector3 cOut = Vector3.ProjectOnPlane(vUp.normalized, Vector3.up);
        float rY = 0f;
        if (cOut.magnitude > 0.001f)
        {
            rY = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(cOut.normalized, Vector3.forward));
            if (Vector3.Cross(cOut.normalized, Vector3.forward).y > 0)
            {
                rY *= -1f;
            }
        }

        //Debug.Log(rY);

        float rX = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(vUp, Vector3.up));
        if (rX - TS.climb <= 0f)
        {
            rX = 0f;
        }
        else
        {
            rX -= TS.climb;
        }
        if (rX > TS.lowestAngle)
        {
            rX = TS.lowestAngle;
        }


        Quaternion QUOT = Quaternion.Euler(rX, rY, 0f);

        transform.rotation = QUOT;
        connection.transform.rotation = QUOT;

        transform.localPosition = segLength * (QUOT * Vector3.up) + parent.localPosition;
        connection.transform.localPosition = (transform.localPosition + parent.localPosition) * 0.5f;

        transform.localScale = Vector3.one * bScale;
        Vector3 cScale = Vector3.one * bScale;
        cScale.y = segLength * 0.5f;
        connection.transform.localScale = cScale;

        if (bScale > TS.cutoffScale)
        {
            float splitChance = Random.Range(0f, 1f);

            if (splitChance < TS.splitChance)
            {
                if (TS.split != null)
                {
                    GetComponent<MeshRenderer>().material = TS.split;
                }

                float roll = Random.Range(TS.minimumRoll, TS.maximumRoll);
                //float roll = Random.Range(-180f, 180f);
                float wander = Random.Range(TS.radialNoiseLowerBound, TS.radialNoiseUpperBound);

                Quaternion wOut = Quaternion.Euler(wander, roll, 0f);

                float randSplit = Random.Range(TS.minimumTrunkSeparation, TS.maximumTrunkSeparation);
                Quaternion sub;
                float randSep = Random.Range(TS.splitAngleUpperBound, TS.splitAngleLowerBound);
                sub = Quaternion.Euler(randSplit * randSep, roll, 0f);

                Quaternion Accum = QUOT * wOut * sub;

                TreeNode TN = Instantiate(TS.nodeType);
                TN.recursiveGeneration(AN, TS, bScale * Mathf.Sqrt(1f - randSplit), 
                    transform, Accum * Vector3.up, depth + 1);

                sub = Quaternion.Euler((randSplit - 1f) * randSep, roll, 0f);
                Accum = QUOT * wOut * sub;

                TN = Instantiate(TS.nodeType);
                TN.recursiveGeneration(AN, TS, bScale * Mathf.Sqrt(randSplit), 
                    transform, Accum * Vector3.up, depth + 1);
            }
            else
            {
                if (TS.joint != null)
                {
                    GetComponent<MeshRenderer>().material = TS.joint;
                }

                float roll = Random.Range(TS.minimumRoll, TS.maximumRoll);
                //float roll = Random.Range(-180f, 180f);
                float wander = Random.Range(TS.radialNoiseLowerBound, TS.radialNoiseUpperBound);

                Quaternion wOut = Quaternion.Euler(wander, roll, 0f);
                Quaternion Accum = QUOT * wOut;

                TreeNode TN = Instantiate(TS.nodeType);
                TN.recursiveGeneration(AN, TS, bScale, transform, 
                    Accum * Vector3.up, depth + 1);
            }
        }

        transform.SetParent(parent);
        connection.transform.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
