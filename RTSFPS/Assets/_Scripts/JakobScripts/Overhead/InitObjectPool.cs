using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitObjectPool
{
    public System.Type sysType { get; private set; }

    public InitObjectPool(System.Type objType)
    {
        sysType = objType;
    }

    List<InitializableObject> objList;
    public List<InitializableObject> ObjList
    {
        get { if (objList == null) { objList = new List<InitializableObject>(); } return objList; }
        protected set { objList = value; }
    }
}
