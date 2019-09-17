using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoList <T> where T : InitializableObject
{
    public static bool ProtectedAdd(List<T> list, T elem)
    {
        if (elem == null)
            return false;
        if (list.Contains(elem))
            return true;
        list.Add(elem);
        return true;
    }

    public static bool ProtectedRemove(List<T> list, T elem)
    {
        if (elem == null)
            return false;
        return list.Remove(elem);
    }

    public static List<T> AutoMake(ref List<T> list)
    {
        if (list == null)
            list = new List<T>();
        return list;
    }
}
