using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CommandMovement : AbstractCommand<CommandMovement, PermitMovement, Controller, Body>
{
    public List<UnitDirectional> direction;

    protected override void FeedPermission(PermitMovement p)
    {
        for (int j = 0; j < direction.Count; ++j)
        {
            if (ChannelTypes.CheckTypes(direction[j].channel, p.channel))
            {
                if (direction[j].module != null)
                {
                    p.SetDirection(direction[j].module.direction);
                }
            }
        }
    }
}
