using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FireStats
{
    public float damage = 1f;
    public float range = 100f;
    public float speed = 20f;
}

public class Bullet : InitializableObject
{
    public float ConeAngle = 5f;
    public FireStats bulletStats;

    FireStats gunStats;
    Vector3 origin;
    Vector3 direction;

    float maxDist;

    public AnimationCurve accuracyCalc;

    public void SetBulletStats(FireStats fs, Vector3 pos, Quaternion dir)
    {
        gunStats = fs;
        origin = pos;
        transform.position = pos;
        transform.rotation = dir;
        maxDist = bulletStats.range + gunStats.range;
        direction = dir * Quaternion.Euler(accuracyCalc.Evaluate(Random.Range(0, 1) * ConeAngle), Random.Range(0, 360f), 0) * Vector3.up;
    }

    private void FixedUpdate()
    {
        if (maxDist <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            RaycastHit[] rhit = Physics.RaycastAll(origin, direction, Mathf.Min((gunStats.speed + bulletStats.speed) * Time.fixedDeltaTime, maxDist));

            if (rhit.Length > 0)
            {
                RaycastHit closest = rhit[0];
                for (int i = 1; i < rhit.Length; ++i)
                {
                    if (rhit[i].distance < closest.distance)
                    {
                        closest = rhit[i];
                    }
                }

                Body b = closest.collider.GetComponent<Body>();
                if (b != null)
                {
                    b.Damage(bulletStats.damage + gunStats.damage);
                }
                else
                    Destroy(gameObject);

                transform.position += direction * (gunStats.speed + bulletStats.speed) * Time.fixedDeltaTime;
            }
            else
            {
                transform.position += direction * (gunStats.speed + bulletStats.speed) * Time.fixedDeltaTime;

                rhit = Physics.RaycastAll(origin, -direction, Mathf.Min((gunStats.speed + bulletStats.speed) * Time.fixedDeltaTime, maxDist));

                if (rhit.Length > 0)
                {
                    RaycastHit closest = rhit[0];
                    for (int i = 1; i < rhit.Length; ++i)
                    {
                        if (rhit[i].distance < closest.distance)
                        {
                            closest = rhit[i];
                        }
                    }

                    Body b = closest.collider.GetComponent<Body>();
                    if (b != null)
                    {
                        b.Damage(bulletStats.damage + gunStats.damage);
                    }
                    else
                        Destroy(gameObject);
                }
            }
        }

        maxDist -= (gunStats.speed + bulletStats.speed) * Time.fixedDeltaTime;
    }
}
