using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSManagers;
using RTSUI;

public class QueueButton : MonoBehaviour
{
    public int queuePlace = 0;

    public void OnClick()
    {
        if (SelectionManager.Instance.PrimarySelectable.type == EntityType.Barracks)
        {

            if (queuePlace == 0)
            {
                SelectionManager.Instance.PrimarySelectable.gameObject.GetComponent<Barracks>().currentBuildTime = 0;
                ResourceManager.Instance.Refund(EntityType.Droid);
            }
            else {
                SelectionManager.Instance.PrimarySelectable.gameObject.GetComponent<Barracks>().buildTimes.Dequeue();
                ResourceManager.Instance.Refund(EntityType.Droid);
            }
        }
        else {
            Debug.Log("Queue Button Selection was not valid");
        }
    }
}
