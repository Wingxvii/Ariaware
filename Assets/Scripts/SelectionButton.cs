using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTSUI
{
    public class SelectionButton : MonoBehaviour
    {
        public EntityType prefabType;
        public SelectableObject parentObject;

        public void OnCreate(SelectableObject parentObj)
        {
            parentObject = parentObj;
            prefabType = parentObj.type;
        }

        public void OnClick()
        {
            SelectionUI.Instance.OnElementSelected(this);
        }

    }
}