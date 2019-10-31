using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using netcodeRTS;

public class Player : SelectableObject
{
    public Transform playerTransform;
    public Rigidbody playerBody;

    public int activeWeapon = 0;

    static public Vector3 pos = new Vector3(0, 0, 0);
    public float moveSpeed = 1;
    public float maxSpeed = 20.0f;

    // Start is called before the first frame update
    protected override void BaseStart()
    {
        currentHealth = 200;
        maxHealth = 200;

        destructable = false;

        playerTransform = this.GetComponent<Transform>();
        playerBody = this.GetComponent<Rigidbody>();

    }

    public void SendWeapon(int weaponNum) {
        activeWeapon = weaponNum;
    }

    public void SendUpdate(Vector3 pos, Vector3 rot, int state) {
        Debug.Log("Recieved");
    }

    // Update is called once per frame
    protected override void BaseUpdate()
    {
        if (Input.GetKey(KeyCode.D))
        {
            playerBody.velocity += new Vector3(1 * moveSpeed, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerBody.velocity += new Vector3(1 * -moveSpeed, 0, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            playerBody.velocity += new Vector3(0, 0, 1 * moveSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerBody.velocity += new Vector3(0, 0, 1 * -moveSpeed);
        }
    }

    protected override void BaseFixedUpdate()
    {
        if (playerBody.velocity.magnitude > maxSpeed)
        {
            playerBody.velocity = playerBody.velocity.normalized * maxSpeed;
        }
    }

    public override void OnDeath()
    {
        Debug.Log("Player's Dead");
        if (ResourceConstants.UNKILLABLEPLAYER)
        {
            this.currentHealth = 200;
        }
        else
        {
            base.OnDeath();
        }
    }

    public override void OnDamage(int num, SelectableObject culprit)
    {
        NetworkManager.SendDamagePlayer(num, this.id + 1, culprit.id);
        base.OnDamage(num, culprit);
    }
}
