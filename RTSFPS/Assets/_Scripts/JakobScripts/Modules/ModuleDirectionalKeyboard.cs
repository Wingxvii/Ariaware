using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleDirectionalKeyboard : ModuleDirectional
{
    public KeyType forward = KeyType.W;
    public KeyType back = KeyType.S;
    public KeyType left = KeyType.A;
    public KeyType right = KeyType.D;
    public KeyType up = KeyType.None;
    public KeyType down = KeyType.None;

    protected override Vector3 GetDirectionalInput()
    {
        Vector3 output = Vector3.zero;

        if (Input.GetKey(Key(forward)))
            output += Vector3.forward;
        if (Input.GetKey(Key(left)))
            output += Vector3.left;
        if (Input.GetKey(Key(back)))
            output += Vector3.back;
        if (Input.GetKey(Key(right)))
            output += Vector3.right;
        if (Input.GetKey(Key(up)))
            output += Vector3.up;
        if (Input.GetKey(Key(down)))
            output += Vector3.down;

        return output;
    }
}
