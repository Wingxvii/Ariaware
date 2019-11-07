using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurretState
{
    Idle,
    IdleShooting,
    PositionalShooting,
    TargetedShooting,
    Recoil,
    Reloading,
}


public class Turret : SelectableObject
{
    public TurretState state = TurretState.Idle;
    public float visionRange = 30.0f;
    public float maxRange = 50.0f;
    private Player attackPoint;
    private Vector3 attackTarget;
    public float shortestDist;
    public ParticleSystem muzzle;

    //stats
    public float reloadRate = 5.0f;
    public float recoilRate = 0.5f;
    public int attackAmno = 10;
    public int attackDamage = 5;

    public float accuracy = 0.85f;

    public float reloadTimer = 0.0f;
    public int currentAmno = 10;

    //rotation
    public float rotateSpeed;
    public Vector3 faceingPoint = new Vector3(0, 0, 0);

    //hit ray
    private RaycastHit hit;
    public LayerMask turretLayerMask;

    protected override void BaseStart()
    {
        muzzle = GetComponentInChildren<ParticleSystem>();
        currentHealth = 500;
        maxHealth = 500;
        turretLayerMask = LayerMask.GetMask("Player");
        turretLayerMask += LayerMask.GetMask("Wall");

    }
    protected override void BaseFixedUpdate()
    {
        if (state != TurretState.Idle)
        {
            //calculate rotations
            var q = Quaternion.LookRotation(faceingPoint - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed);
        }

        shortestDist = float.MaxValue;

        switch (state)
        {
            case TurretState.Idle:
                //search for shortest player
                foreach (Player player in DroidManager.Instance.playerTargets)
                {
                    float dist = Vector3.Distance(player.transform.position, this.transform.position);
                    //Debug.Log(dist);
                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        attackPoint = player;
                        //Debug.Log("Seen");
                    }
                }
                if (shortestDist < maxRange)
                {
                    state = TurretState.IdleShooting;
                }
                break;
            case TurretState.IdleShooting:
                //search for shortest player
                foreach (Player player in DroidManager.Instance.playerTargets)
                {
                    float dist = Vector3.Distance(player.transform.position, this.transform.position);
                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        attackPoint = player;
                    }
                }
                if (shortestDist < maxRange)
                {
                    state = TurretState.IdleShooting;
                    faceingPoint = attackPoint.transform.position;
                    if (currentAmno > 0)
                    {
                        muzzle.Play();
                        if (!HitWall() && accuracy - (shortestDist / 100.0f) > Random.Range(0.0f, 1.0f))
                        {
                            attackPoint.OnDamage(attackDamage, this);
                        }
                        currentAmno--;
                        state = TurretState.Recoil;
                        reloadTimer += recoilRate;

                    }
                    else
                    {
                        reloadTimer += reloadRate;
                        state = TurretState.Reloading;
                    }
                }

                break;
            case TurretState.TargetedShooting:
                //search for shortest player
                foreach (Player player in DroidManager.Instance.playerTargets)
                {
                    float dist = Vector3.Distance(player.transform.position, this.transform.position);
                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        attackPoint = player;
                    }
                }

                //look at
                if (shortestDist < maxRange)
                {
                    faceingPoint = attackPoint.transform.position;

                    if (currentAmno > 0)
                    {
                        muzzle.Play();

                        if (!HitWall() && accuracy - (shortestDist / 100.0f) > Random.Range(0.0f, 1.0f))
                        {
                            attackPoint.OnDamage(attackDamage, this);
                        }
                        currentAmno--;
                        state = TurretState.Recoil;
                        reloadTimer += recoilRate;

                    }
                    else
                    {
                        reloadTimer += reloadRate;
                        state = TurretState.Reloading;
                    }

                }
                else
                {
                        state = TurretState.Idle;
                }

                break;
            case TurretState.Recoil:
                //search for shortest player
                foreach (Player player in DroidManager.Instance.playerTargets)
                {
                    float dist = Vector3.Distance(player.transform.position, this.transform.position);
                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        attackPoint = player;
                    }
                }

                //look at
                faceingPoint = attackPoint.transform.position;

                if (reloadTimer <= 0.0f)
                {
                    state = TurretState.IdleShooting;
                }
                break;

            case TurretState.Reloading:
                if (reloadTimer <= 0.0f)
                {
                    currentAmno = 10;
                    state = TurretState.Idle;
                }
                break;
        }
    }

    protected override void BaseUpdate()
    {
        if (reloadTimer >= 0.0f)
        {
            reloadTimer -= Time.deltaTime;
        }
    }
    public void IssueAttack(Player attackee)
    {
        Debug.Log("attack issueds");
        if (state != TurretState.Reloading)
        {
            state = TurretState.TargetedShooting;
        }
        attackPoint = attackee;
    }
    public void Reload() {
        reloadTimer += reloadRate;
        state = TurretState.Reloading;
    }
    public override void OnDeath()
    {
        Debug.Log("Dead Turret");
        base.OnDeath();
    }


    private bool HitWall() {
        if (Physics.Raycast(this.transform.position, (attackPoint.transform.position - this.transform.position), out hit, maxRange, turretLayerMask))
        {
            if (hit.transform.gameObject.tag == "SelectableObject" && hit.transform.GetComponent<SelectableObject>().type == EntityType.Wall)
            {
                Debug.Log("Hit Wall");
                hit.transform.GetComponent<Wall>().WallIsHit(hit.point);
                hit.transform.GetComponent<Wall>().OnDamage(attackDamage, this);
                return true;
            }
            else if (hit.transform.gameObject.tag == "SelectableObject" && hit.transform.GetComponent<SelectableObject>().type == EntityType.Player) {
                Debug.Log("Hit Player");
                return false;
            }
            else
            {
                Debug.Log("Hit Something Else");
                Debug.Log(hit.transform.gameObject.name);
                return false;
            }

        }

        return false;

    }

}
