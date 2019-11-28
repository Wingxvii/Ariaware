using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLoc : MonoBehaviour
{
    public bool created = false;

    static List<SpawnLoc> locations;
    public static List<SpawnLoc>Locations
    {
        get { if (locations == null) { locations = new List<SpawnLoc>(); } return locations; }
    }

    private void Awake()
    {
        CreateMe();
    }

    public void CreateMe()
    {
        if (!created)
        {
            created = true;
            Locations.Add(this);
        }
    }
}
