using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DroidState {
    Standing = 0,
    Moving = 1,
    AttackMoving = 2,
    TetherAttacking = 3,
    TargetAttacking = 4,
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


    protected override void BaseStart()
    {

        selfRigid = this.GetComponent<Rigidbody>();
        currentHealth = 100;
        maxHealth = 100;

    }

    protected override void BaseUpdate()
    {
        if (state != DroidState.Standing) { 
            //calculate rotations
            var q = Quaternion.LookRotation(faceingPoint - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed);
        } 

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
        state = DroidState.TargetAttacking;
        attackPoint = attackee;
    }

    public override void OnDeath()
    {
        Debug.Log("Dead droid");
        DroidManager.Instance.KillDroid(this);
        base.OnDeath();
    }

    //unique classes
    public void MoveTo(Vector2 pos) {
        faceingPoint = journeyPoint;

        Vector2 dir = new Vector2(pos.x - this.transform.position.x, pos.y - this.transform.position.z).normalized;
        selfRigid.velocity = new Vector3(dir.x, 0, dir.y) * maxSpeed;
    }
    private void OnAttack() {
        if (currentCoolDown <= 0.0f) {
            attackPoint.OnDamage(attackDamage, this);
            currentCoolDown = coolDown;
        }
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

}
