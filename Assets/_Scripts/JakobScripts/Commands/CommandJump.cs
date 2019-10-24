using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CommandJump : AbstractCommand<CommandJump, PermitJump, Controller, Body>
{
    public List<UnitButton> jumpButton;

    protected override void FeedPermission(PermitJump p)
    {   
        for (int j = 0; j < jumpButton.Count; ++j)
        {
            if (ChannelTypes.CheckTypes(jumpButton[j].channel, p.channel))
            {
                if (jumpButton[j].module != null && jumpButton[j].module.pressed)
                {
                    p.Jump();
                }
            }
        }
    }
}
