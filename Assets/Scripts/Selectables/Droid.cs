﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DroidState {
    Standing = 0,
    Moving = 1,
    AttackMoving = 2,
    TetherAttacking = 3,
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
    protected override void BaseStart()
    {
        selfRigid = this.GetComponent<Rigidbody>();
        currentHealth = 100;
        maxHealth = 100;

    }

    protected override void BaseUpdate()
    {
        //tick attack cooldown
        if (currentCoolDown > 0.0f) {
            currentCoolDown -= Time.deltaTime;
        }
    }

    //overrided base classes
    protected override void BaseFixedUpdate()
    {
        if (selfRigid.velocity.magnitude > maxSpeed)
        {
            selfRigid.velocity = selfRigid.velocity.normalized * maxSpeed;
        }

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
            case DroidState.TetherAttacking:
                //check if gameobject is seeable
                journeyPoint = attackPoint.transform.position;
                MoveTo(new Vector2(journeyPoint.x, journeyPoint.z));
                break;
            case DroidState.AttackMoving:
                //check if gameobject is seeable
                MoveTo(new Vector2(journeyPoint.x, journeyPoint.z));
                if (Vector3.Distance(DroidManager.Instance.playerTarget.transform.position, this.transform.position) < visualRange)
                {
                    state = DroidState.TetherAttacking;
                    attackPoint = DroidManager.Instance.playerTarget;
                }
                break;
            case DroidState.Standing:
                if (Vector3.Distance(DroidManager.Instance.playerTarget.transform.position, this.transform.position) < visualRange)
                {
                    state = DroidState.TetherAttacking;
                    attackPoint = DroidManager.Instance.playerTarget;
                }
                break;

        }

    }

    public override void OnDeactivation()
    {
        OnDeath();
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
        state = DroidState.TetherAttacking;
        attackPoint = attackee;
    }

    public override void OnDeath()
    {
        Debug.Log("Dead droid");
        DroidManager.Instance.KillDroid(this);
    }

    //unique classes
    public void MoveTo(Vector2 pos) {

        Vector2 dir = new Vector2(pos.x - this.transform.position.x, pos.y - this.transform.position.z).normalized;
        selfRigid.velocity = new Vector3(dir.x, 0, dir.y) * maxSpeed;
    }
    private void OnAttack() {
        if (currentCoolDown <= 0.0f) {
            attackPoint.OnDamage(attackDamage);
            currentCoolDown = coolDown;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "SelectableObject" && other.gameObject.GetComponent<SelectableObject>().type == EntityType.Player)
        {
            state = DroidState.TetherAttacking;
            attackPoint = other.gameObject.GetComponent<Player>();
            OnAttack();
        }
    }

}