using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadGameState : ReadBase
{
    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            AddFirst();

            return true;
        }

        return false;
    }

    protected override void First()
    {
        
    }
}
