using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ChannelTypes
{
    public ChannelName cName = ChannelName.None;
    public int cNum = 0;
    public bool allChannelNums = false;
}

public enum ChannelName
{
    None,
    Weapons,
    Effects,
    Misc,
    All
}