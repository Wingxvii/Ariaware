using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractManager
{
    private List<BasePACES> uniqueObjects;
    protected List<BasePACES> UniqueObjects
    {
        get
        {
            if (UniqueObjects == null)
                UniqueObjects = new List<BasePACES>();
            return UniqueObjects;
        }
        private set { UniqueObjects = value; }
    }

    //bool initialized = false;
    //public bool Initialized
    //{
    //    get { return initialized; }
    //    protected set { initialized = value; }
    //}
    //
    //public void Initialize()
    //{
    //    Initialized = true;
    //}
}
