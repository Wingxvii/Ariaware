﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ResourceConstants {
    public const int SUPPLY_MAX = 100;
    public const int COST_BARRACKS = 500;
    public const int COST_DROIDS = 200;
    public const int COST_TURRERT = 400;
    public const int COST_WALL = 250;

    public const int SUPPLY_PER_BARRACKS = 20;

    public const bool CREDITS_OFF = true;
    public const bool UNKILLABLEPLAYER = true;

}

public class ResourceManager : MonoBehaviour
{
    #region SingletonCode
    private static ResourceManager _instance;
    public static ResourceManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public int credits = 0;
    public int totalSupply = 0;
    public int supplyCurrent = 0;

    public int numBarracksActive = 0;

    public Text creditText;
    public Text supplyText;
    public Text time;

    public float timeElapsed;

    // Start is called before the first frame update
    void Start()
    {
        credits = 1000;
        timeElapsed = 0;
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        time.text = ((int)(timeElapsed / 60.0f)).ToString("00") + ":" + ((int)(timeElapsed % 60)).ToString("00");
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (ResourceConstants.CREDITS_OFF)
        {
            credits = 99999;
        }
        else {
            credits += 1;
        }

        creditText.text = credits.ToString();
        supplyText.text = supplyCurrent.ToString() + "/" + totalSupply.ToString();

    }

    public bool Purchase(EntityType type) {
        switch (type) {
            case EntityType.Barracks:
                if (credits >= ResourceConstants.COST_BARRACKS)
                {
                    credits -= ResourceConstants.COST_BARRACKS;
                    return true;
                }
                return false;
                break;
            case EntityType.Droid:
                if (credits >= ResourceConstants.COST_DROIDS)
                {
                    credits -= ResourceConstants.COST_DROIDS;
                    return true;
                }
                return false;
                break;
            case EntityType.Turret:
                if (credits >= ResourceConstants.COST_TURRERT)
                {
                    credits -= ResourceConstants.COST_TURRERT;
                    return true;
                }
                return false;
                break;
            case EntityType.Wall:
                if (credits >= ResourceConstants.COST_WALL)
                {
                    credits -= ResourceConstants.COST_WALL;
                    return true;
                }
                return false;
                break;

            default:
                Debug.Log("PURCHACE ERROR");
                return false;
        }
    }
    public void UpdateSupply() {
        totalSupply = numBarracksActive * ResourceConstants.SUPPLY_PER_BARRACKS;
        if (totalSupply > ResourceConstants.SUPPLY_MAX) {
            totalSupply = ResourceConstants.SUPPLY_MAX;
        }
    }
}