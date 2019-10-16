using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CommandRotation : AbstractCommand<CommandRotation, PermitRotation, Controller, Puppet>
{
    public List<UnitDirectional> axes;

    protected override void FeedPermission(PermitRotation p)
    {
        for (int j = 0; j < axes.Count; ++j)
        {
            if (ChannelTypes.CheckTypes(axes[j].channel, p.channel))
            {
                if (axes[j].module != null)
                {
                    p.AddRotation(axes[j].module.direction);
                }
            }
        }
    }
}
