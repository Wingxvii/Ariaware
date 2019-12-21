using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSManagers;
public class ShellPlacement : MonoBehaviour
{
    public Material red;
    public Material green;

    public Collider selfCollider;
    public MeshRenderer selfRenderer;

    public bool placeable = false;

    public int collisionCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        selfCollider = this.GetComponent<Collider>();
        selfRenderer = this.GetComponent<MeshRenderer>();
    }

    void OnEnable() {
        collisionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //checks for placeability
        if (collisionCount == 0) {
            placeable = true;
            selfRenderer.material = green;
        }
    }


    private void OnTriggerEnter(Collider collision)
    {
        collisionCount++;
        selfRenderer.material = red;
        placeable = false;
    }

    public void OnTriggerExit(Collider collision)
    {
        collisionCount--;
    }
}
