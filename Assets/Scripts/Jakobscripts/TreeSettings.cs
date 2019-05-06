using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TreeSettings : MonoBehaviour
{
    public TreeNode nodeType;
    public TreeNode connection;
    public Material joint;
    public Material split;

    public float initialScale = 1f;
    public float cutoffScale = 0.4f;
    public float decay = 0.1f;

    public float splitChance = 1f;
    public float splitAngleUpperBound = 60f;
    public float splitAngleLowerBound = 30f;

    public float radialNoiseUpperBound = 15f;
    public float radialNoiseLowerBound = -15f;

    public float climb = 3f;
    public float lowestAngle = 100f;

    public float segmentLengthUpperBound = 4.0f;
    public float segmentLengthLowerBound = 1.0f;

    //public float segmentScaleWeight = 1f;

    public float maximumTrunkSeparation = 0.5f;
    public float minimumTrunkSeparation = 0.5f;

    public float maximumRoll = 180f;
    public float minimumRoll = -180f;
}
