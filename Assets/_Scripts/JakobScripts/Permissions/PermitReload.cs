using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermitReload : AbstractPermission<PermitReload, CommandReload, PACES.Gun, Controller>
{
    public float cooldownTime = 2f;
    float timeElapsed = 0f;

    Animator anim;
    bool reload = false;

    public void SetReload()
    {
        if (timeElapsed <= 0f && !reload)
        {
            //Debug.Log("RELOAD");
            reload = true;
        }
    }

    protected override bool CrossBranchInitialize()
    {
        if (base.CrossBranchInitialize())
        {
            anim = SpecificActor.Container.GetObj(0).GetComponent<BodySlot>().EntityPlug.GetObj(0).GetComponent<Animator>();

            return true;
        }

        return false;
    }

    protected override void CrossBranchDeInitialize()
    {
        base.CrossBranchDeInitialize();
    }

    protected override void FeedPuppet()
    {
        if (reload && timeElapsed <= 0f)
        {
            timeElapsed = cooldownTime;
            if (anim != null)
                anim.Play("Reload");

            AmmoClip ac = SpecificActor.ammo.GetObj(0);
            ac.SetBullets(ac.maxBulletCount);

            SpecificActor.pauseShooting = cooldownTime;
            SpecificActor.pauseTotal = cooldownTime;
            SpecificActor.CurrentInventory.GetObj(0).pauseSwap = cooldownTime;
        }
        else if (timeElapsed > 0f)
        {
            timeElapsed -= Time.fixedDeltaTime;
            if (timeElapsed <= 0f)
                reload = false;
        }
    }
}
