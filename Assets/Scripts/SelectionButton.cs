using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionButton : MonoBehaviour
{
    public EntityType prefabType;
    public SelectableObject parentObject;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCreate(SelectableObject parentObj) {
        parentObject = parentObj;
        prefabType = parentObj.type;
    }

    public void OnClick()
    {
        SelectionUI.Instance.OnElementSelected(this);
    }

}
