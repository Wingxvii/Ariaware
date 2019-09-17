using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EType<T> where T : Object
{
    public static T FindType(Object Base)
    {
        if (Base.GetType().IsSubclassOf(typeof(T)) || Base.GetType() == typeof(T))
            return (T)Base;
        return null;
    }

    public static T ProtectedInstantiate (T obj, Transform parent = null)
    {
        T obj2 = FindType(Object.Instantiate(obj));

        return obj2;
    }
}

public abstract class FType
{
    public static bool FindIfType(Object Base, System.Type t)
    {
        if (Base.GetType().IsSubclassOf(t) || Base.GetType() == t)
            return true;
        return false;
    }
}