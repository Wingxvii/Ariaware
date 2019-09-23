using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermitInvDebugOutput : AbsInvPermission<PermitInvDebugOutput, CommandInvDebugOutput, Inventory, Controller>
{
    bool putLog = false;

    public void OutputLog(bool outputlog)
    {
        putLog = outputlog;
    }

    protected override void FeedPuppet()
    {
        if (putLog)
        {
            Debug.Log("YES!");
            putLog = false;
        }
    }
}
