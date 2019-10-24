using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EType<T> where T : Object
{
    public static T FindType(Object Base)
    {
        if (Base == null)
            return null;
        if (Base.GetType().IsSubclassOf(typeof(T)) || Base.GetType() == typeof(T))
            return (T)Base;
        return null;
    }

    //public static T Downcast(Object Base)
    //{
    //    if (Base == null)
    //        return null;
    //    if (typeof(T).IsSubclassOf(Base.GetType()) || Base.GetType() == typeof(T))
    //        return (T)Base;
    //    return null;
    //}

    public static T ProtectedInstantiate (T obj, Transform parent = null)
    {
        T obj2 = FindType(Object.Instantiate(obj));

        return obj2;
    }
}

public abstract class FType
{
    public static bool FindIfType(Object Derived, System.Type Base)
    {
        if (Derived == null)
            return false;
        if (Derived.GetType().IsSubclassOf(Base) || Derived.GetType() == Base)
            return true;
        return false;
    }

    public static bool FindIfType(System.Type Derived, System.Type Base)
    {
        if (Derived.IsSubclassOf(Base) || Derived == Base)
            return true;
        return false;
    }
}