using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ByteTools
{

    //Sending type tools
    public static char[] ParseByte(float data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        return new char[4] { (char)bytes[0], (char)bytes[1], (char)bytes[2], (char)bytes[3] };
    }
    public static char[] ParseByte(Vector3 data)
    {
        byte[] bytes = BitConverter.GetBytes(data.x);
        byte[] bytes2 = BitConverter.GetBytes(data.y);
        byte[] bytes3 = BitConverter.GetBytes(data.z);

        return new char[12] { (char)bytes[0], (char)bytes[1], (char)bytes[2], (char)bytes[3], (char)bytes[4], (char)bytes[5], (char)bytes[6], (char)bytes[7], (char)bytes[8], (char)bytes[9], (char)bytes[10], (char)bytes[11] };
    }
    public static char[] ParseByte(int data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        return new char[4] { (char)bytes[0], (char)bytes[1], (char)bytes[2], (char)bytes[3] };
    }
    //Sending types with string return
    public static string ParseBytetoString(float data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        return "" + (char)bytes[0] + (char)bytes[1] + (char)bytes[2] + (char)bytes[3];
    }
    public static string ParseBytetoString(Vector3 data)
    {
        byte[] bytes = BitConverter.GetBytes(data.x);
        byte[] bytes2 = BitConverter.GetBytes(data.y);
        byte[] bytes3 = BitConverter.GetBytes(data.z);
        return "" + (char)bytes[0] + (char)bytes[1] + (char)bytes[2] + (char)bytes[3] + (char)bytes2[0] + (char)bytes2[1] + (char)bytes2[2] + (char)bytes2[3] + (char)bytes3[0] + (char)bytes3[1] + (char)bytes3[2] + (char)bytes3[3];
    }
    public static string ParseBytetoString(int data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        return "" + (char)bytes[0] + (char)bytes[1] + (char)bytes[2] + (char)bytes[3];
    }

    //Recieving types
    public static float ToByteFloat(string s)
    {
        byte[] bytes = { (byte)s[0], (byte)s[1], (byte)s[2], (byte)s[3] };
        return BitConverter.ToSingle(bytes, 0);
    }
    public static Vector3 ToByteVector(string s)
    {
        byte[] bytes = { (byte)s[0], (byte)s[1], (byte)s[2], (byte)s[3] };
        byte[] bytes2 = { (byte)s[4], (byte)s[5], (byte)s[6], (byte)s[7] };
        byte[] bytes3 = { (byte)s[8], (byte)s[9], (byte)s[10], (byte)s[11] };

        return new Vector3(BitConverter.ToSingle(bytes, 0), BitConverter.ToSingle(bytes2, 0), BitConverter.ToSingle(bytes3, 0));
    }
    public static int ToByteInt(string s)
    {
        byte[] bytes = { (byte)s[0], (byte)s[1], (byte)s[2], (byte)s[3] };
        return BitConverter.ToInt32(bytes, 0);
    }
}
