﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSManagers;
using netcodeRTS;
using GlobalSettings;
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
    public float shortestDist;
    public ParticleSystem muzzle;

    //stats
    public float reloadRate = 5.0f;
    public float recoilRate = 0.5f;
    public int attackAmno = 10;
    public int attackDamage = 5;

    public float reloadTimer = 0.0f;
    public int currentAmno = 10;

    //rotation
    public float rotateSpeed;
    public Vector3 faceingPoint = new Vector3(0, 0, 0);

    //hit ray
    private RaycastHit hit;
    public LayerMask turretLayerMask;

    //models
    public GameObject head;
    public GameObject body;

    //position update dirty flag
    public bool positionUpdated = false;

    public bool changedToIdle = false;

    public int fixedTimeStep;


    protected override void BaseStart()
    {
        fixedTimeStep = (int)(1f / Time.fixedDeltaTime);

        muzzle = GetComponentInChildren<ParticleSystem>();
        currentHealth = 500;
        maxHealth = 500;
        turretLayerMask = LayerMask.GetMask("Player");
        turretLayerMask += LayerMask.GetMask("Wall");

    }
    protected override void BaseEnable()
    {
        currentHealth = 500;
        positionUpdated = false;

        changedToIdle = false;
    }

    void TickUpdate()
    {
        if (positionUpdated) {
            NetworkManager.AddDataToStack(id, head.transform.rotation.eulerAngles, (int)state);
            changedToIdle = true;
        }
        else if (changedToIdle)
        {
            NetworkManager.AddDataToStack(id, head.transform.rotation.eulerAngles, (int)state);
            changedToIdle = false;
        }

        positionUpdated = false;


    }

    protected override void BaseFixedUpdate()
    {
        
        shortestDist = float.MaxValue;
        float dist = 0.0f;

        #region Fixed Tick
        //count down
        --fixedTimeStep;

        //tick is called 10 times per 50 updates
        if (fixedTimeStep % Setting.FRAMETICK == 0)
        {
            TickUpdate();
        }

        //reset the clock
        if (fixedTimeStep <= 0)
        {
            //updates 50Hz
            fixedTimeStep = (int)(1f / Time.fixedDeltaTime);
        }
        #endregion

        switch (state)
        {
            case TurretState.Idle:
                //search for shortest player
                foreach (Player player in DroidManager.Instance.playerTargets)
                {
                    dist = Vector3.Distance(player.transform.position, this.transform.position);

                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        attackPoint = player;
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
                    dist = Vector3.Distance(player.transform.position, this.transform.position);
                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        attackPoint = player;
                    }
                }
                if (shortestDist < maxRange)
                {

                    //tell networking to send updated data
                    positionUpdated = true;

                    state = TurretState.IdleShooting;
                    faceingPoint = attackPoint.transform.position;
                    if (currentAmno > 0)
                    {
                        muzzle.Play();
                        if (HitPlayer())
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
                dist = Vector3.Distance(attackPoint.transform.position, this.transform.position);
                shortestDist = dist;

                //look at
                if (shortestDist < maxRange)
                {
                    faceingPoint = attackPoint.transform.position;

                    //tell networking to send updated data
                    positionUpdated = true;

                    if (currentAmno > 0)
                    {
                        muzzle.Play();

                        if (HitPlayer())
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
                    dist = Vector3.Distance(player.transform.position, this.transform.position);
                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        attackPoint = player;
                    }
                }

                //look at
                faceingPoint = attackPoint.transform.position;
                //tell networking to send updated data
                positionUpdated = true;

                if (reloadTimer <= 0.0f)
                {
                    ///BUG: If recoiling from TargettedShooting, will go to IdleShooting
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
        if (state != TurretState.Idle)
        {
            Vector3 targetDir = new Vector3(faceingPoint.x - head.transform.position.x, faceingPoint.y - head.transform.position.y, faceingPoint.z - head.transform.position.z);

            // The step size is equal to speed times frame time.
            float step = rotateSpeed * Time.deltaTime;

            Vector3 newDir = Vector3.RotateTowards(head.transform.forward, targetDir, step, 0.0f);


            // Move our position a step closer to the target.
            head.transform.rotation = Quaternion.LookRotation(newDir);
            body.transform.rotation = Quaternion.LookRotation(new Vector3(newDir.x, 0, newDir.z).normalized);

        }

        if (reloadTimer >= 0.0f)
        {
            reloadTimer -= Time.deltaTime;
        }
    }
    public void IssueAttack(Player attackee)
    {
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

    private bool HitPlayer() {
        if (Physics.Raycast(head.transform.position, head.transform.forward , out hit, maxRange, turretLayerMask))
        {
            if (hit.transform.gameObject.tag == "SelectableObject" && hit.transform.GetComponent<SelectableObject>().type == EntityType.Wall)
            {
                hit.transform.GetComponent<Wall>().WallIsHit(hit.point);
                hit.transform.GetComponent<Wall>().OnDamage(attackDamage, this);
                return false;
            }
            else if (hit.transform.gameObject.tag == "SelectableObject" && hit.transform.GetComponent<SelectableObject>().type == EntityType.Player) {
                return true;
            }
            else
            {
                return false;
            }

        }

        return false;

    }

}
