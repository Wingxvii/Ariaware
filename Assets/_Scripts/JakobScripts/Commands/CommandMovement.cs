using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CommandMovement : AbstractCommand<CommandMovement, PermitMovement, Controller, Body>
{
    public ModuleDirectional direction;

    protected override void FeedPermission(PermitMovement p)
    {
        if (p != null && direction != null)
            p.SetDirection(direction.GetDirectionalInput());
    }
}
