using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleAxisKeyboard : ModuleAxis
{
    public KeyType increase = KeyType.None;
    public KeyType decrease = KeyType.None;

    protected override float GetAxis()
    {
        float output = 0f;

        if (Input.GetKey(Key(increase)))
            output += 1f;
        if (Input.GetKey(Key(decrease)))
            output -= 1f;

        return output;
    }
}
