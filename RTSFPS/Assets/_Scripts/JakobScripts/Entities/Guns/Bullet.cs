using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FireStats
{
    public float damage = 1f;
    public float range = 100f;
    public float speed = 20f;

    public float ConeAngle = 5f;
}

public class Bullet : InitializableObject
{
    public FireStats bulletStats;

    FireStats gunStats;
    Vector3 origin;
    Vector3 direction;
    Body ignoreThis;

    float maxDist;

    public void SetBulletStats(FireStats fs, Vector3 pos, Quaternion dir, Body ignore, AnimationCurve acc)
    {
        gunStats = fs;
        origin = pos;
        transform.position = pos;
        Quaternion newDir = dir * Quaternion.Euler(acc.Evaluate(Random.Range(0f, 1f)) * (fs.ConeAngle + gunStats.ConeAngle), Random.Range(0, 360f), 0);
        transform.rotation = newDir;
        maxDist = bulletStats.range + gunStats.range;
        direction = newDir * Vector3.up;
        ignoreThis = ignore;
    }

    private void FixedUpdate()
    {
        float bulletDistance = (gunStats.speed + bulletStats.speed) * Time.fixedDeltaTime;
        origin = transform.position;

        if (maxDist <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            if (ignoreThis != null)
            {
                ignoreThis.EnableColliders(false);
            }

            RaycastHit[] rhit = Physics.RaycastAll(origin, direction, Mathf.Min(bulletDistance, maxDist));

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

                EntityContainer ec = closest.collider.GetComponentInParent<EntityContainer>();
                if (ec != null)
                {
                    for (int i = 0; i < ec.AttachedSlots.Amount; ++i)
                    {
                        SlotBase sb = ec.AttachedSlots.GetObj(i);
                        if (FType.FindIfType(sb.GetSlotType(), typeof(Body)) && sb.BranchInit())
                        {
                            Body b = EType<Body>.FindType(sb.EntityPlug.GetObj(0));
                            if (b != null && b.TreeInit())
                            {
                                b.Damage(bulletStats.damage + gunStats.damage);
                            }

                            break;
                        }
                    }
                }

                Destroy(gameObject);

                transform.position += direction * bulletDistance;
            }
            else
            {
                transform.position += direction * bulletDistance;

                origin = transform.position;

                rhit = Physics.RaycastAll(origin, -direction, Mathf.Min(bulletDistance, maxDist));

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

                    Debug.Log(closest.collider.name);

                    EntityContainer ec = closest.collider.GetComponentInParent<EntityContainer>();
                    if (ec != null)
                    {
                        for (int i = 0; i < ec.AttachedSlots.Amount; ++i)
                        {
                            SlotBase sb = ec.AttachedSlots.GetObj(i);
                            if (FType.FindIfType(sb.GetSlotType(), typeof(Body)) && sb.BranchInit())
                            {
                                Body b = EType<Body>.FindType(sb.EntityPlug.GetObj(0));
                                if (b != null && b.TreeInit())
                                {
                                    b.Damage(bulletStats.damage + gunStats.damage);
                                }

                                break;
                            }
                        }
                    }

                    Destroy(gameObject);
                }
            }

            if (ignoreThis != null)
            {
                ignoreThis.EnableColliders(true);
            }
        }

        maxDist -= bulletDistance;
    }
}
