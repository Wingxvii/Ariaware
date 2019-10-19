using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTarget : MonoBehaviour
{
    public EntityContainer toSpawn;
    public KeyCode spawnButton;

    private void Awake()
    {
        Collider[] cols = GetComponents<Collider>();
        for (int i = 0; i < cols.Length; ++i)
        {
            cols[i].enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(spawnButton) && toSpawn != null)
        {
            EntityContainer eRef = Instantiate(toSpawn);

            eRef.transform.position = RandomPosition();
        }
    }

    Vector3 RandomPosition()
    {
        float x = Random.Range(transform.localScale.x, -transform.localScale.x) * 0.5f + transform.position.x;
        float y = Random.Range(transform.localScale.y, -transform.localScale.y) * 0.5f + transform.position.y;
        float z = Random.Range(transform.localScale.z, -transform.localScale.z) * 0.5f + transform.position.z;

        return new Vector3(x, y, z);
    }
}
