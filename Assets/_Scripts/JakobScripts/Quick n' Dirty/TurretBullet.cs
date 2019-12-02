using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    TrailRenderer aTrail;
    public float timeActive = 0.1f;
    float timeToHit = 0f;
    bool sent = false;

    private void Awake()
    {
        aTrail = GetComponentInChildren<TrailRenderer>();

        if (aTrail != null)
        {
            aTrail.time = timeActive;
        }
    }

    private void FixedUpdate()
    {
        float longBoi = 1000f;

        if (!sent)
        {
            sent = true;

            RaycastHit[] rchit = Physics.RaycastAll(new Ray(transform.position, transform.forward), longBoi);

            if (rchit.Length > 0)
            {
                RaycastHit closest = rchit[0];
                for (int i = 1; i < rchit.Length; ++i)
                {
                    if (closest.distance > rchit[i].distance)
                    {
                        closest = rchit[i];
                    }
                }

                longBoi = closest.distance;
            }

            transform.position = transform.position + longBoi * transform.forward;
        }

        timeToHit += Time.fixedDeltaTime;

        if (timeToHit >= timeActive)
        {
            Destroy(gameObject);
        }
    }
}
