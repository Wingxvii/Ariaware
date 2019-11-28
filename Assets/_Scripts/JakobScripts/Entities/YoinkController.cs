using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoinkController : Empty
{
    protected override bool CreateVars()
    {
        if(base.CreateVars())
        {
            CtrlHookEvent.FireEvent(new CtrlHookEvent(this));
            return true;
        }

        return false;
    }
}
