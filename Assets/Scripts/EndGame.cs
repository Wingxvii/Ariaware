using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSManagers;

public class EndGame : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SelectableObject" && other.gameObject.GetComponent<SelectableObject>().type == EntityType.Player) {
            RTSGameManager.Instance.GameEndLose();
        }
    }

}
