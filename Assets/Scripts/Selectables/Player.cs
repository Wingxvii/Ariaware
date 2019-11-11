using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using netcodeRTS;
using RTSManagers;

public class Player : SelectableObject
{
    public Transform playerTransform;
    public Rigidbody playerBody;

    public int activeWeapon = 0;
    public Animator anim;

    static public Vector3 pos = new Vector3(0, 0, 0);
    public float moveSpeed = 1;
    public float maxSpeed = 20.0f;

    public GameObject[] weapons;

    // Start is called before the first frame update
    protected override void BaseStart()
    {
        currentHealth = 200;
        maxHealth = 200;

        destructable = false;

        playerTransform = this.GetComponent<Transform>();
        playerBody = this.GetComponent<Rigidbody>();

        foreach(GameObject weapon in weapons) {
            weapon.SetActive(false);
        }
        weapons[0].SetActive(true);
    }

    public void SendWeapon(int weaponNum) {
        weapons[activeWeapon].SetActive(false);
        activeWeapon = weaponNum;
        weapons[activeWeapon].SetActive(true);
    }

    public void SendUpdate(Vector3 pos, Vector3 rot, int state) {
        Debug.Log("Force Added");
        this.GetComponent<Rigidbody>().velocity = (pos - this.transform.position) * 10f;
        this.transform.rotation = Quaternion.Euler(new Vector3(0f, rot.y, 0f));


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

        //Debug.Log(Vector3.Project(this.GetComponent<Rigidbody>().velocity.normalized, transform.forward).magnitude);

        anim.SetFloat("Walk", Vector3.Dot(this.GetComponent<Rigidbody>().velocity, transform.forward) / 10);
        anim.SetFloat("Turn", Vector3.Dot(this.GetComponent<Rigidbody>().velocity, transform.right) / 10);

    }



    /*
    protected override void BaseFixedUpdate()
    {
        if (playerBody.velocity.magnitude > maxSpeed)
        {
            playerBody.velocity = playerBody.velocity.normalized * maxSpeed;
        }
    }
    */
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
