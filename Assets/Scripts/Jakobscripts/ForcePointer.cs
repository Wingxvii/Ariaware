using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ForcePointer : MonoBehaviour
{
    float radius = 0f;
    Vector3 position = Vector3.zero;
    Vector3 basePosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetBasePosition(Vector3 pos)
    {
        basePosition = pos;
    }

    public void SetForceLoc(Vector3 pos)
    {
        if (pos.magnitude > 0)
            position = pos.normalized;
        else
            position = Vector3.zero;
    }

    public void SetRadius(float r)
    {
        radius = r;
    }

    public void UpdateLocation()
    {
        transform.localPosition = position * radius + basePosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
