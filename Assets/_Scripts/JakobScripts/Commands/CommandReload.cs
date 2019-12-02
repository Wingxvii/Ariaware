using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandReload : AbstractCommand<CommandReload, PermitReload, Controller, PACES.Gun>
{
    public List<UnitButton> reloadButton;

    protected override void FeedPermission(PermitReload p)
    {
        for (int j = 0; j < reloadButton.Count; ++j)
        {
            if (ChannelTypes.CheckTypes(reloadButton[j].channel, p.channel))
            {
                if (reloadButton[j].module != null && reloadButton[j].module.pressed)
                {
                    //Debug.Log("RELOAD");
                    p.SetReload();
                }
            }
        }
    }
}
