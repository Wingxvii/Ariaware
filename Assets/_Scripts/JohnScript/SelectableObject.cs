using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SelectableObject : MonoBehaviour
{
    public Behaviour halo;
    public int level = 1;
    public int id;
    public BuildingEnum type;
    public bool deletable = true;

    // Start is called before the first frame update
    void Start()
    {
        halo = (Behaviour)this.GetComponent("Halo");
        halo.enabled = false;
    }

    public void OnSelect()
    {
        Debug.Log("Selected");
        if (halo != null) { halo.enabled = true; }
    }
    public void OnDeselect()
    {
        Debug.Log("Deselected");
        halo.enabled = false;
    }
}
