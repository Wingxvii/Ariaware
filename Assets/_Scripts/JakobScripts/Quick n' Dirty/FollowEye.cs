using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEye : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position = ModuleDirectionalTOBII.LERPpos;
    }
}
