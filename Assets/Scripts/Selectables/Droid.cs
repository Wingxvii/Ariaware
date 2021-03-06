﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using netcodeRTS;
using RTSManagers;

public enum DroidState {
    Standing = 0,
    Moving = 1,
    AttackMoving = 2,
    TetherAttacking = 3,
    TargetAttacking = 4,
    Dancing = 5,
}

public class Droid : SelectableObject
{
    public float maxSpeed = 5.0f;
    public float minSpeed = 2.0f;
    private Rigidbody selfRigid;

    private Vector3 journeyPoint;
    private Player attackPoint;
    public DroidState state = DroidState.Standing;

    public float journeyAccuracy = 5.0f;

    public int attackDamage = 5;
    public float coolDown = 1.0f;
    public float currentCoolDown = 0.0f;

    public float visualRange = 20.0f;

    //rotation
    public float rotateSpeed;
    public Vector3 faceingPoint = new Vector3(0, 0, 0);

    //models
    public GameObject body;

    //animation
    public Animator anim;

    private bool deathCheck = false;

    protected override void BaseStart()
    {

        selfRigid = this.GetComponent<Rigidbody>();
        maxHealth = 100;
        anim = this.GetComponent<Animator>();
        currentHealth = 100;
        deathCheck = false;

    }

    protected override void BaseEnable()
    {
        currentHealth = 100;
        deathCheck = false;
    }

    protected override void BaseUpdate()
    {
        //tick attack cooldown
        if (currentCoolDown > 0.0f) {
            currentCoolDown -= Time.deltaTime;
        }

        if (state != DroidState.Standing)
        {
            Vector3 targetDir = new Vector3(faceingPoint.x - transform.position.x, 0, faceingPoint.z - transform.position.z);

            // The step size is equal to speed times frame time.
            float step = rotateSpeed * Time.deltaTime;

            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);

            // Move our position a step closer to the target.
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    //overrided base classes
    protected override void BaseFixedUpdate()
    {
        if (selfRigid.velocity.magnitude > maxSpeed)
        {
            selfRigid.velocity = selfRigid.velocity.normalized * maxSpeed;
        }

        float shortestDist;

        //AI STATE MACHINE
        switch (state) {
            case DroidState.Moving:
                if (Vector3.Distance(this.transform.position, journeyPoint) < journeyAccuracy)
                {
                    state = DroidState.Standing;
                }
                else
                {

                    MoveTo(new Vector2(journeyPoint.x, journeyPoint.z));
                   
                }
                break;
            case DroidState.TargetAttacking:
                //check if gameobject is seeable
                journeyPoint = attackPoint.transform.position;
                MoveTo(new Vector2(journeyPoint.x, journeyPoint.z));
                break;

            case DroidState.TetherAttacking:
                shortestDist = float.MaxValue;

                //check shortest in range for each player
                foreach (Player player in DroidManager.Instance.playerTargets)
                {
                    float dist = Vector3.Distance(player.transform.position, this.transform.position);
                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        attackPoint = player;
                    }
                }
                if (shortestDist < visualRange)
                {
                    state = DroidState.TetherAttacking;
                }


                //check if gameobject is seeable
                journeyPoint = attackPoint.transform.position;
                MoveTo(new Vector2(journeyPoint.x, journeyPoint.z));
                break;
            case DroidState.AttackMoving:
                //check if gameobject is seeable
                MoveTo(new Vector2(journeyPoint.x, journeyPoint.z));
                shortestDist = float.MaxValue;

                //check shortest in range for each player
                foreach (Player player in DroidManager.Instance.playerTargets) {
                    float dist = Vector3.Distance(player.transform.position, this.transform.position);
                    if (dist < shortestDist) {
                        shortestDist = dist;
                        attackPoint = player;
                    }
                }
                if (shortestDist < visualRange)
                {
                    state = DroidState.TetherAttacking;
                }

                break;
            case DroidState.Standing:
                shortestDist = float.MaxValue;

                anim.SetFloat("Walk", 0);
                anim.SetFloat("Turn", 0);

                foreach (Player player in DroidManager.Instance.playerTargets)
                {
                    float dist = Vector3.Distance(player.transform.position, this.transform.position);
                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        attackPoint = player;
                    }
                }
                if (shortestDist < visualRange)
                {
                    state = DroidState.TetherAttacking;
                }


                break;

        }

    }

    public override void IssueLocation(Vector3 location)
    {
        state = DroidState.Moving;
        journeyPoint = location;
    }

    public void IssueAttack(Vector3 location)
    {
        state = DroidState.AttackMoving;
        journeyPoint = location;
    }
    public void IssueAttack(Player attackee)
    {
        state = DroidState.TargetAttacking;
        attackPoint = attackee;
    }

    public override void OnDeath()
    {
        if (!deathCheck)
        {
            deathCheck = true;
            StartCoroutine("PlayDeath");
        }
    }

    //unique classes
    public void MoveTo(Vector2 pos) {
        faceingPoint = journeyPoint;

        Vector2 dir = new Vector2(pos.x - this.transform.position.x, pos.y - this.transform.position.z).normalized * maxSpeed;
        selfRigid.velocity = new Vector3(dir.x, selfRigid.velocity.y, dir.y) ;

        anim.SetFloat("Walk", Vector3.Dot(selfRigid.velocity, transform.forward));
        anim.SetFloat("Turn", Vector3.Dot(selfRigid.velocity, transform.right));

    }
    private void OnAttack() {
        if (currentCoolDown <= 0.0f) {
            attackPoint.OnDamage(attackDamage, this);
            currentCoolDown = coolDown;
            anim.Play("Attack");
        }
    }


    private void OnDance() {
        anim.Play("Dance");

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "SelectableObject" && other.gameObject.GetComponent<SelectableObject>().type == EntityType.Player)
        {
            if (state != DroidState.AttackMoving && state != DroidState.Moving && state != DroidState.TargetAttacking) {
                state = DroidState.TetherAttacking;
            }if (!(state == DroidState.TetherAttacking && attackPoint != other.gameObject.GetComponent<Player>()) && state != DroidState.TargetAttacking) {
                attackPoint = other.gameObject.GetComponent<Player>();
            }
            OnAttack();
        }
    }

    IEnumerator PlayDeath() {
        anim.Play("Death");
        NetworkManager.SendKilledEntity(this);

        yield return new WaitForSeconds(3.0f);

        Debug.Log("Dead droid");
        DroidManager.Instance.KillDroid(this);
        SelectionManager.Instance.DeselectItem(this);
    }
}
