using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this uses the factory method
public enum BuildingEnum
{
    None,
    Barracks,
    Upgrades,
    Turrert,
    Wall,
    Trap,
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

public class Barracks : Building
{
    public Barracks(Vector3 pos, out GameObject pointer) : base()
    {
        pos = new Vector3(pos.x, pos.y, pos.z);
        self = GameObject.Instantiate(BuildingManager.Instance.buildingPrefabs[1], pos, Quaternion.identity);
        self.GetComponent<SelectableObject>().id = this.id;
        pointer = self;
    }
}

public class Upgrades : Building
{
    public Upgrades(Vector3 pos, out GameObject pointer) : base()
    {
        pos = new Vector3(pos.x, pos.y + 1.33f, pos.z);
        self = GameObject.Instantiate(BuildingManager.Instance.buildingPrefabs[2], pos, Quaternion.identity);
        self.GetComponent<SelectableObject>().id = this.id;
        pointer = self;
    }
}

public class Turrert : Building
{
    public Turrert(Vector3 pos, out GameObject pointer) : base()
    {
        pos = new Vector3(pos.x, pos.y + 2, pos.z);
        self = GameObject.Instantiate(BuildingManager.Instance.buildingPrefabs[3], pos, Quaternion.identity);
        self.GetComponent<SelectableObject>().id = this.id;
        pointer = self;
    }
}

public class Wall : Building
{
    public Wall(Vector3 pos, out GameObject pointer) : base()
    {
        pos = new Vector3(pos.x, pos.y + 2, pos.z);
        self = GameObject.Instantiate(BuildingManager.Instance.buildingPrefabs[4], pos, Quaternion.identity);
        self.GetComponent<SelectableObject>().id = this.id;
        pointer = self;
    }
}

public class Trap : Building
{
    public Trap(Vector3 pos, out GameObject pointer) : base()
    {
        pos = new Vector3(pos.x, pos.y + 2, pos.z);
        self = GameObject.Instantiate(BuildingManager.Instance.buildingPrefabs[5], pos, Quaternion.identity);
        self.GetComponent<SelectableObject>().id = this.id;
        pointer = self;
    }
}