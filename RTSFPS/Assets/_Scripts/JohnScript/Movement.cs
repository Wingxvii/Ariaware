using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class Movement : MonoBehaviour
{

    public Transform playerTransform;
    public Rigidbody playerBody;

    static public Vector3 pos = new Vector3(0,0,0);
    public float moveSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = this.GetComponent<Transform>();
        playerBody = this.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D)) {
            playerBody.velocity += new Vector3(1 * moveSpeed,0,0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerBody.velocity += new Vector3(1 * -moveSpeed, 0, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            playerBody.velocity += new Vector3(0, 0,1 * moveSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerBody.velocity += new Vector3(0,0,1 * -moveSpeed);
        }
    }
}
