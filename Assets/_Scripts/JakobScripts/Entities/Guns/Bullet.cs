using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using netcodeRTS;

[System.Serializable]
public class FireStats
{
    public int damage = 1;
    public float range = 100f;
    public float speed = 20f;

    public float ConeAngle = 5f;
}

public class Bullet : UpdateableObject
{
    public FireStats bulletStats;
    public bool useHitscan = false;
    bool usingHitscan = false;

    public float timeActive = 0.1f;
    float timeToHit = 0f;
    bool hasReached = false;

    GunVector accuser;
    FireStats gunStats;
    Vector3 origin;
    Vector3 direction;
    Body ignoreThis;

    TrailRenderer aTrail;

    float maxDist;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddFixedUpdate();

            //Prevent a possible error for if it's changed in editor while firing
            usingHitscan = useHitscan;
            aTrail = GetComponentInChildren<TrailRenderer>();

            if (aTrail != null)
            {
                aTrail.time = timeActive;
            }

            return true;
        }

        return false;
    }

    protected override bool InnerInitialize()
    {
        if (base.InnerInitialize())
        {


            return true;
        }

        return false;
    }

    protected override void InnerDeInitialize()
    {


        base.InnerDeInitialize();
    }

    public void SetBulletStats(FireStats fs, GunVector culprit, Body ignore, AnimationCurve acc)
    {
        gunStats = fs;
        origin = culprit.transform.position;
        transform.position = origin;
        Quaternion newDir = culprit.transform.rotation * Quaternion.Euler(acc.Evaluate(Random.Range(0f, 1f)) * (bulletStats.ConeAngle + gunStats.ConeAngle), Random.Range(0, 360f), 0);
        transform.rotation = newDir;
        maxDist = bulletStats.range + gunStats.range;
        direction = newDir * Vector3.up;
        ignoreThis = ignore;
        accuser = culprit;
    }

    protected override void FixedUpdateObject()
    //private void FixedUpdate()
    {
        if (usingHitscan)
            HitscanBullet();
        else
            RaycastBullet();
    }

    void RaycastBullet()
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
                    if (ec.ID > 3)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(1);
                        sb.Append(",");
                        sb.Append(bulletStats.damage + gunStats.damage);
                        sb.Append(",");
                        sb.Append(ec.ID - 1);
                        sb.Append(",");
                        NET_PACKET.NetworkDataManager.SendNetData((int)PacketType.DAMAGEDEALT, sb.ToString());
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
                        if (rhit[i].distance > closest.distance)
                        {
                            closest = rhit[i];
                        }
                    }

                    EntityContainer ec = closest.collider.GetComponentInParent<EntityContainer>();
                    if (ec != null)
                    {
                        if (ec.ID > 3)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append(1);
                            sb.Append(",");
                            sb.Append(bulletStats.damage + gunStats.damage);
                            sb.Append(",");
                            sb.Append(ec.ID - 1);
                            sb.Append(",");
                            NET_PACKET.NetworkDataManager.SendNetData((int)PacketType.DAMAGEDEALT, sb.ToString());
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

    void HitscanBullet()
    {
        if (!hasReached)
        {
            if (ignoreThis != null)
            {
                ignoreThis.EnableColliders(false);
            }

            float place = maxDist;
            RaycastHit[] rhit = Physics.RaycastAll(origin, direction, maxDist);

            if (rhit.Length > 0)
            {

                RaycastHit closest = rhit[0];

                for (int i = 1; i < rhit.Length; ++i)
                {
                    if (closest.distance > rhit[i].distance)
                    {
                        closest = rhit[i];
                    }
                }

                place = closest.distance;

                EntityContainer ec = closest.collider.GetComponentInParent<EntityContainer>();

                if (ec != null)
                {
                    if (ec.ID > 3)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(1);
                        sb.Append(",");
                        sb.Append(bulletStats.damage + gunStats.damage);
                        sb.Append(",");
                        sb.Append(ec.ID - 1);
                        sb.Append(",");
                        NET_PACKET.NetworkDataManager.SendNetData((int)PacketType.DAMAGEDEALT, sb.ToString());
                    }
                }
            }

            if (ignoreThis != null)
            {
                ignoreThis.EnableColliders(true);
            }

            hasReached = true;
            transform.position += direction * place;
        }

        timeToHit += Time.fixedDeltaTime;

        if (timeToHit >= timeActive)
        {
            Destroy(gameObject);
        }
    }
}
