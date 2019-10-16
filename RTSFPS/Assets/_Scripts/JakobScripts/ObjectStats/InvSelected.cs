using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class InvSelected : InvStat
{


    public int Selected { get; protected set; } = 0;

    public void SetNew(int selected)
    {
        
    }
}
