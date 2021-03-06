﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTSManagers;

public class Barracks : SelectableObject
{
    public Slider buildProcess;
    public Queue<float> buildTimes;
    public float currentBuildTime = 0;

    public static float droidTrainTime = 5.0f;
    public static int maxTrainingCap = 25;

    public Canvas canvas;

    public GameObject flagObj;
    public bool flagActive = false;

    public Transform spawnPoint;


    //inherited function realizations
    protected override void BaseStart()
    {
        currentHealth = 1000;
        maxHealth = 1000;

        canvas = GetComponentInChildren<Canvas>();
        canvas.transform.LookAt(canvas.transform.position + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.up);

        buildProcess = canvas.transform.Find("Building Progress").GetComponent<Slider>();
        buildProcess.gameObject.SetActive(false);

        buildTimes = new Queue<float>();
        
        //create a flag from the prefab
        flagObj = Instantiate(flagObj, Vector3.zero, Quaternion.identity);
        flagObj.SetActive(false);
    }

    protected override void BaseEnable()
    {
        currentHealth = 1000;
        flagActive = false;

    }

    protected override void BaseUpdate()
    {

        //add to queue
        if (currentBuildTime <= 0 && buildTimes.Count > 0)
        {
            buildProcess.gameObject.SetActive(true);
            currentBuildTime += buildTimes.Dequeue();
        }
        //tick queue
        else if (currentBuildTime > 0)
        {
            buildProcess.value = currentBuildTime / droidTrainTime;
            currentBuildTime -= Time.deltaTime;
            if (currentBuildTime <= 0)
            {
                if (flagActive)
                {
                    DroidManager.Instance.QueueFinished(spawnPoint, EntityType.Droid, flagObj.transform.position);
                }
                else
                {
                    DroidManager.Instance.QueueFinished(spawnPoint, EntityType.Droid);
                }
            }
        }
        //queue ended
        else if (currentBuildTime <= 0) {
            buildProcess.gameObject.SetActive(false);
        }

        //flag activation on selection
        if (selected && SelectionManager.Instance.PrimarySelectable == this && flagActive) {
            flagObj.SetActive(true);
        }
        else if(flagObj.activeSelf){
            flagObj.SetActive(false);
        }

    }

    public override void IssueLocation(Vector3 location) {
        flagObj.transform.position = new Vector3(location.x, location.y + 2.5f, location.z);
        flagActive = true;
    }
    public override void OnActivation()
    {
        ResourceManager.Instance.numBarracksActive++;
        ResourceManager.Instance.UpdateSupply();
    }
    public override void OnDeactivation()
    {
        ResourceManager.Instance.numBarracksActive--;
        ResourceManager.Instance.UpdateSupply();

        while(buildTimes.Count > 0) {
            buildTimes.Dequeue();
            ResourceManager.Instance.Refund(EntityType.Droid);
        }

        currentBuildTime = 0;
        flagActive = false;
        buildTimes.Clear();
    }

    //child-sepific functions
    public void OnTrainRequest() {
        if (ResourceManager.Instance.Purchase(EntityType.Droid) && buildTimes.Count < maxTrainingCap)
        {
            buildTimes.Enqueue(DroidManager.Instance.RequestQueue(EntityType.Droid));
            dll.UserMetrics.DroidIncrease();
        }
        else if (buildTimes.Count >= maxTrainingCap)
        {
            Debug.Log("QUEUE IS FULL");
        }
        else {
            Debug.Log("NOT ENOUGH CREDITS");
        }
    }

    public override void OnDeath()
    {


        base.OnDeath();
    }

}
