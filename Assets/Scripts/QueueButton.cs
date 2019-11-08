using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSManagers;

public class QueueButton : MonoBehaviour
{
    public int queuePlace = 0;

    public void OnClick()
    {
        if (SelectionManager.Instance.PrimarySelectable.type == EntityType.Barracks)
        {

            if (queuePlace == 19)
            {
                SelectionManager.Instance.PrimarySelectable.gameObject.GetComponent<Barracks>().currentBuildTime = SelectionManager.Instance.PrimarySelectable.gameObject.GetComponent<Barracks>().buildTimes.Dequeue();
                ResourceManager.Instance.Purchase(EntityType.Droid);

            }
            else {
                SelectionManager.Instance.PrimarySelectable.gameObject.GetComponent<Barracks>().buildTimes.Dequeue();            }
        }
        else {
            Debug.Log("Queue Button Selection was not valid");
        }
    }
}
