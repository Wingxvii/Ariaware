using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvCycle : AbsInvCommand<CommandInvCycle, PermitInvCycle, Controller, Inventory>
{
    public ModuleAxis Cycle;
    float threshold = 0.5f;
    bool invert = false;

    protected override void FeedPermission(PermitInvCycle p)
    {
        if (p != null && Cycle != null)
        {
            float axisVal = Cycle.GetAxis();
            axisVal /= Mathf.Abs(axisVal);
            if (axisVal > Mathf.Abs(threshold))
                p.ReceiveInput((invert ? 1 : -1));
            else if (axisVal < -Mathf.Abs(threshold))
                p.ReceiveInput((invert ? -1 : 1));
            else
                p.ReceiveInput(0);
        }
    }
}
