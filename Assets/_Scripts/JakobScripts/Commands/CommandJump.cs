using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CommandJump : AbstractCommand<CommandJump, PermitJump, Controller, Body>
{
    public ModuleButton jumpButton;

    protected override void FeedPermission(PermitJump p)
    {
        if (p != null && jumpButton != null)
        {
            if (jumpButton.pressed)
                p.Jump();
        }
    }
}
