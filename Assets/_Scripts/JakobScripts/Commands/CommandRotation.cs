using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CommandRotation : AbstractCommand<CommandRotation, PermitRotation, Controller, Puppet>
{
    public ModuleDirectional axes;

    protected override void FeedPermission(PermitRotation p)
    {
        if (p != null && axes != null)
            p.AddRotation(axes.GetDirectionalInput());
    }
}
