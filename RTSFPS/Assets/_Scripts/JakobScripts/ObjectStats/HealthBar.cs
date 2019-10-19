using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Body))]
public class HealthBar : ObjectStat
{
    Body bodyRef = null;
    public float maxHealth = 10f;
    public float health = 0f;
    Slider slide = null;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            health = maxHealth;

            slide = GetComponentInChildren<Slider>();

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {
            bodyRef = EType<Body>.FindType(Ent.GetObj(0));

            return true;
        }

        return false;
    }

    protected override void InnerDeInitialize()
    {
        bodyRef = null;

        base.InnerDeInitialize();
    }

    protected override void DestroyVars()
    {
        health = 0f;

        slide = null;

        base.DestroyVars();
    }

    private void Update()
    {
        if (slide != null)
        {
            slide.value = health / maxHealth;
        }
    }

    public void Damage(float dam)
    {
        health -= dam;
        if (health <= 0f)
        {
            health = 0f;
            EntityContainer HIT = bodyRef.Container.GetObj(0);
            if (HIT != null)
            {
                HIT.gameObject.SetActive(false);
                Destroy(HIT.gameObject);
            }
        }
    }

}
