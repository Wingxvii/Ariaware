using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvDebugOutput : AbsInvCommand<CommandInvDebugOutput, PermitInvDebugOutput, Controller, Inventory>
{
    public ModuleButton debugButton;

    protected override void FeedPermission(PermitInvDebugOutput p)
    {
        if (debugButton != null && p != null)
        {
            p.OutputLog(debugButton.pressed);
        }
    }
}
