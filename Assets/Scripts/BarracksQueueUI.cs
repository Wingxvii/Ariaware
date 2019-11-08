using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTSUI;

public class BarracksQueueUI : MonoBehaviour
{

    public GameObject ButtonPrefab;
    public Slider buildProcess;
    public GameObject UIParent;

    public float XStart = 210;
    public float XOffset = -15;
    public int numOfCols = 20;

    public float YStart = -6.5f;
    public float YOffset = 0;
    public int numOfRows = 1;

    public List<GameObject> buttonPool;


    // Start is called before the first frame update
    void Start()
    {            
        //fill positions in slot
        for (int counter2 = 0; counter2 < numOfRows; counter2++)
        {
            for (int counter = 0; counter < numOfCols; counter++)
            {
                Vector3 pos = new Vector3(XStart + (counter * XOffset), YStart + (counter2 * YOffset), 0.0f);
                buttonPool.Add(GameObject.Instantiate(ButtonPrefab, pos + UIParent.transform.position, Quaternion.identity));
                buttonPool[buttonPool.Count].GetComponent<QueueButton>().queuePlace = counter;
            }
        }
        
        foreach (GameObject button in buttonPool)
        {
            
            button.transform.SetParent(UIParent.transform);
            button.gameObject.SetActive(false);
        }
    }


}
