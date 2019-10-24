using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvDebugOutput : AbstractCommand<CommandInvDebugOutput, PermitInvDebugOutput, Controller, Inventory>
{
    public List<UnitButton> debugButton;

    protected override void FeedPermission(PermitInvDebugOutput p)
    {
        for (int i = 0; i < debugButton.Count; ++i)
        {
            if (ChannelTypes.CheckTypes(debugButton[i].channel, p.channel))
            if (debugButton[i].module != null)
            {
                p.OutputLog(debugButton[i].module.pressed);
            }
        }
    }
}
