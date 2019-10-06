using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this uses the factory method
public enum BuildingEnum
{
    None,
    RedBuilding,
    BlueBuilding,
    GreenBuilding,
    YellowBuilding
}


public abstract class Building
{
    //creates and sets an id for each building
    public Building()
    {
        id = ++idtracker;
    }
    public GameObject self;
    public static int idtracker { get; private set; }
    public int id;
}

public class RedBuilding : Building
{
    public RedBuilding(Vector3 pos, out GameObject pointer) : base()
    {
        pos = new Vector3(pos.x, pos.y, pos.z);
        self = GameObject.Instantiate(CommandPattern.Instance.redFab, pos, Quaternion.identity);
        self.GetComponent<SelectableObject>().id = this.id;
        pointer = self;
    }
}

public class BlueBuilding : Building
{
    public BlueBuilding(Vector3 pos, out GameObject pointer) : base()
    {
        pos = new Vector3(pos.x, pos.y + 1.33f, pos.z);
        self = GameObject.Instantiate(CommandPattern.Instance.blueFab, pos, Quaternion.identity);
        self.GetComponent<SelectableObject>().id = this.id;
        pointer = self;
    }
}

public class GreenBuilding : Building
{
    public GreenBuilding(Vector3 pos, out GameObject pointer) : base()
    {
        pos = new Vector3(pos.x, pos.y + 2, pos.z);
        self = GameObject.Instantiate(CommandPattern.Instance.greenFab, pos, Quaternion.identity);
        self.GetComponent<SelectableObject>().id = this.id;
        pointer = self;
    }
}

public class YellowBuilding : Building
{
    public YellowBuilding(Vector3 pos, out GameObject pointer) : base()
    {
        pos = new Vector3(pos.x, pos.y + 2, pos.z);
        self = GameObject.Instantiate(CommandPattern.Instance.yellowFab, pos, Quaternion.identity);
        self.GetComponent<SelectableObject>().id = this.id;
        pointer = self;
    }
}