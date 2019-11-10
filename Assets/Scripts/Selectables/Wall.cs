﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSManagers;

public class Wall : SelectableObject
{
    public ParticleSystem Wallhit;

    protected override void BaseStart()
    {
        Wallhit = GetComponentInChildren<ParticleSystem>();

        currentHealth = 500;
        maxHealth = 500;

    }

    public void WallIsHit(Vector3 hitPoint) {
        Wallhit.transform.position = hitPoint;
        Wallhit.Play();
    }
    public override void OnDeath()
    {
        Debug.Log("Dead Wall");
        SelectionManager.Instance.deactivatedObjects[2].Enqueue(this);
        base.OnDeath();
    }


}
