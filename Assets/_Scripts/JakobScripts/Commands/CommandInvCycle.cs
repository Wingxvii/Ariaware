using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvCycle : AbstractCommand<CommandInvCycle, PermitInvCycle, Controller, Inventory>//AbsInvCommand<CommandInvCycle, PermitInvCycle, Controller, Inventory>
{
    public List<UnitAxis> Cycle;
    float threshold = 0.5f;
    bool invert = false;

    protected override void FeedPermission(PermitInvCycle p)
    {
        for (int i = 0; i < Cycle.Count; ++i)
        {

            if (ChannelTypes.CheckTypes(Cycle[i].channel, p.channel))
            {
                if (Cycle[i].module != null)
                {
                    float axisVal = Cycle[i].module.value;
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
    }
}
