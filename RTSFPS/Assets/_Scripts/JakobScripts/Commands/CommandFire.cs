using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandFire : AbstractCommand<CommandFire, PermitFire, Controller, Gun>
{
    public List<UnitButton> fireButton;

    protected override void FeedPermission(PermitFire p)
    {
        for (int j = 0; j < fireButton.Count; ++j)
        {
            if (ChannelTypes.CheckTypes(fireButton[j].channel, p.channel))
            {
                if (fireButton[j].module != null)
                {
                    p.Fire(fireButton[j].module.pressed);
                }
            }
        }
    }
}
