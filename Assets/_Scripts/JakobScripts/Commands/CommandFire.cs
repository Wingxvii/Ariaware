using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandFire : AbstractCommand<CommandFire, PermitFire, Controller, Gun>
{
    public List<UnitButton> fireButton;

    protected override void FeedPermission(PermitFire p)
    {
        //Debug.Log(p.name);
        for (int j = 0; j < fireButton.Count; ++j)
        {
            //TestDebug("HEHEHEHEHE", p);
            if (ChannelTypes.CheckTypes(fireButton[j].channel, p.channel))
            {
                //TestDebug("+_+_+_+_+_+_+", p);
                if (fireButton[j].module != null)
                {
                    //TestDebug("<<<<<<<<<<<<<<<<<<<", p);
                    p.Fire(fireButton[j].module.pressed);
                }
            }
        }
    }

    void TestDebug(string deb, PermitFire p)
    {
        if (p.name == "Gun")
        {
            Debug.Log(deb);
        }
    }
}
