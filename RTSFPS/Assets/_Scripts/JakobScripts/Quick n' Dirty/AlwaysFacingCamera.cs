using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFacingCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Camera.allCameras[0].transform.rotation;
    }
}
