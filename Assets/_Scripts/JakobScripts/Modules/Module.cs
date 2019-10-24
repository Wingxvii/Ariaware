using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class Module : InitializableObject
{
    protected KeyCode Key(KeyType kt)
    {
        switch (kt)
        {
            case KeyType.num0:
                return KeyCode.Alpha0;
            case KeyType.num1:
                return KeyCode.Alpha1;
            case KeyType.num2:
                return KeyCode.Alpha2;
            case KeyType.num3:
                return KeyCode.Alpha3;
            case KeyType.num4:
                return KeyCode.Alpha4;
            case KeyType.num5:
                return KeyCode.Alpha5;
            case KeyType.num6:
                return KeyCode.Alpha6;
            case KeyType.num7:
                return KeyCode.Alpha7;
            case KeyType.num8:
                return KeyCode.Alpha8;
            case KeyType.num9:
                return KeyCode.Alpha9;

            case KeyType.A:
                return KeyCode.A;
            case KeyType.B:
                return KeyCode.B;
            case KeyType.C:
                return KeyCode.C;
            case KeyType.D:
                return KeyCode.D;
            case KeyType.E:
                return KeyCode.E;
            case KeyType.F:
                return KeyCode.F;
            case KeyType.G:
                return KeyCode.G;
            case KeyType.H:
                return KeyCode.H;
            case KeyType.I:
                return KeyCode.I;
            case KeyType.J:
                return KeyCode.J;
            case KeyType.K:
                return KeyCode.K;
            case KeyType.L:
                return KeyCode.L;
            case KeyType.M:
                return KeyCode.M;
            case KeyType.N:
                return KeyCode.N;
            case KeyType.O:
                return KeyCode.O;
            case KeyType.P:
                return KeyCode.P;
            case KeyType.Q:
                return KeyCode.Q;
            case KeyType.R:
                return KeyCode.R;
            case KeyType.S:
                return KeyCode.S;
            case KeyType.T:
                return KeyCode.T;
            case KeyType.U:
                return KeyCode.U;
            case KeyType.V:
                return KeyCode.V;
            case KeyType.W:
                return KeyCode.W;
            case KeyType.X:
                return KeyCode.X;
            case KeyType.Y:
                return KeyCode.Y;
            case KeyType.Z:
                return KeyCode.Z;

            case KeyType.UpArrow:
                return KeyCode.UpArrow;
            case KeyType.LeftArrow:
                return KeyCode.LeftArrow;
            case KeyType.DownArrow:
                return KeyCode.DownArrow;
            case KeyType.RightArrow:
                return KeyCode.RightArrow;

            case KeyType.LeftShift:
                return KeyCode.LeftShift;
            case KeyType.RightShift:
                return KeyCode.RightShift;

            case KeyType.Space:
                return KeyCode.Space;

            default:
                return KeyCode.None;
        }
    }
}

public enum KeyType
{
    None,
    num0,
    num1,
    num2,
    num3,
    num4,
    num5,
    num6,
    num7,
    num8,
    num9,
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z,
    UpArrow,
    LeftArrow,
    DownArrow,
    RightArrow,
    LeftShift,
    RightShift,
    Space,

}