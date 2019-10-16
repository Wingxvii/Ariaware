using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleButtonKeyboard : ModuleButton
{
    public KeyType button = KeyType.None;

    protected override bool GetPressed()
    {
        return Input.GetKey(Key(button));
    }
}
