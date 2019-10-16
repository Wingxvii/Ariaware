using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct UnitAxis
{
    public ChannelTypes channel;
    public ModuleAxis module;
}

[Serializable]
public struct UnitButton
{
    public ChannelTypes channel;
    public ModuleButton module;
}

[Serializable]
public struct UnitDirectional
{
    public ChannelTypes channel;
    public ModuleDirectional module;
}

[Serializable]
public class ChannelTypes
{
    public ChannelName cName = ChannelName.All;
    public int cNum = 0;
    public bool allChannelNums = false;

    public static bool CheckTypes(ChannelTypes c1, ChannelTypes c2)
    {
        if (c1.cName == ChannelName.None || c2.cName == ChannelName.None)
            return false;

        if (c1.cName == ChannelName.All || c2.cName == ChannelName.All)
            return true;

        if (c1.cName == c2.cName)
            if (c1.allChannelNums || c2.allChannelNums || c1.cNum == c2.cNum)
                return true;

        return false;
    }
}

public enum ChannelName
{
    All,
    Weapons,
    Effects,
    Misc,
    None
}