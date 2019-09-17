using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empty : Puppet
{
    protected override SlotBase GetSlot()
    {
        return Container.GetObj(0).GetComponent<EmptySlot>();
    }
}
