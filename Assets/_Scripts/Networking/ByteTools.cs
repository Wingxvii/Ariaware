using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public static class ByteTools
{
    //Bit shifting, faster than BitConverter
    const string BITSHIFT_DLL = "Testing Bitshift";
    [DllImport(BITSHIFT_DLL)]
    internal static extern float UtoFloat(uint u);
    [DllImport(BITSHIFT_DLL)]
    internal static extern int UtoInt(uint u);
    [DllImport(BITSHIFT_DLL)]
    internal static extern uint FloatToU(float f);
    [DllImport(BITSHIFT_DLL)]
    internal static extern uint IntToU(int i);

    //Sending type tools
    public static char[] ParseByte(float data)
    {
        //byte[] bytes = BitConverter.GetBytes(data);
        //return new char[4] { (char)bytes[0], (char)bytes[1], (char)bytes[2], (char)bytes[3] };
        uint u = FloatToU(data);
        return new char[4] { (char)((byte)u), (char)((byte)(u >> 8)), (char)((byte)(u >> 16)), (char)((byte)(u >> 24)) };
    }
    public static char[] ParseByte(Vector3 data)
    {
        //byte[] bytes = BitConverter.GetBytes(data.x);
        //byte[] bytes2 = BitConverter.GetBytes(data.y);
        //byte[] bytes3 = BitConverter.GetBytes(data.z);
        //
        //return new char[12] { (char)bytes[0], (char)bytes[1], (char)bytes[2], (char)bytes[3], (char)bytes[4], (char)bytes[5], (char)bytes[6], (char)bytes[7], (char)bytes[8], (char)bytes[9], (char)bytes[10], (char)bytes[11] };
        uint ux = FloatToU(data.x);
        uint uy = FloatToU(data.y);
        uint uz = FloatToU(data.z);
        return new char[12] { (char)((byte)ux), (char)((byte)(ux >> 8)), (char)((byte)(ux >> 16)), (char)((byte)(ux >> 24)),
            (char)((byte)uy), (char)((byte)(uy >> 8)), (char)((byte)(uy >> 16)), (char)((byte)(uz >> 24)),
            (char)((byte)uz), (char)((byte)(uz >> 8)), (char)((byte)(uz >> 16)), (char)((byte)(uz >> 24))

        };
    }
    public static char[] ParseByte(int data)
    {
        //byte[] bytes = BitConverter.GetBytes(data);
        //return new char[4] { (char)bytes[0], (char)bytes[1], (char)bytes[2], (char)bytes[3] };
        uint u = IntToU(data);
        return new char[4] { (char)((byte)u), (char)((byte)(u >> 8)), (char)((byte)(u >> 16)), (char)((byte)(u >> 24)) };
    }
    //Sending types with string return
    public static string ParseBytetoString(float data)
    {
        //byte[] bytes = BitConverter.GetBytes(data);
        //return "" + (char)bytes[0] + (char)bytes[1] + (char)bytes[2] + (char)bytes[3];
        uint u = FloatToU(data);
        return "" + (char)((byte)u) + (char)((byte)(u >> 8)) + (char)((byte)(u >> 16)) + (char)((byte)(u >> 24));
    }
    public static string ParseBytetoString(Vector3 data)
    {
        //byte[] bytes = BitConverter.GetBytes(data.x);
        //byte[] bytes2 = BitConverter.GetBytes(data.y);
        //byte[] bytes3 = BitConverter.GetBytes(data.z);
        //return "" + (char)bytes[0] + (char)bytes[1] + (char)bytes[2] + (char)bytes[3] + (char)bytes2[0] + (char)bytes2[1] + (char)bytes2[2] + (char)bytes2[3] + (char)bytes3[0] + (char)bytes3[1] + (char)bytes3[2] + (char)bytes3[3];
        uint ux = FloatToU(data.x);
        uint uy = FloatToU(data.x);
        uint uz = FloatToU(data.x);
        return "" + (char)((byte)ux) + (char)((byte)(ux >> 8)) + (char)((byte)(ux >> 16)) + (char)((byte)(ux >> 24)) +
            (char)((byte)uy) + (char)((byte)(uy >> 8)) + (char)((byte)(uy >> 16)) + (char)((byte)(uy >> 24)) +
            (char)((byte)uz) + (char)((byte)(uz >> 8)) + (char)((byte)(uz >> 16)) + (char)((byte)(uz >> 24));
    }
    public static string ParseBytetoString(int data)
    {
        //byte[] bytes = BitConverter.GetBytes(data);
        //return "" + (char)bytes[0] + (char)bytes[1] + (char)bytes[2] + (char)bytes[3];
        uint u = IntToU(data);
        return "" + (char)((byte)u) + (char)((byte)(u >> 8)) + (char)((byte)(u >> 16)) + (char)((byte)(u >> 24));
    }

    //Recieving types
    public static float ToByteFloat(string s)
    {
        //byte[] bytes = { (byte)s[0], (byte)s[1], (byte)s[2], (byte)s[3] };
        //return BitConverter.ToSingle(bytes, 0);
        uint u = (uint)s[0] + (((uint)s[1]) << 8) + (((uint)s[2]) << 16) + (((uint)s[3]) << 24);
        return UtoFloat(u);
    }
    public static Vector3 ToByteVector(string s)
    {
        //byte[] bytes = { (byte)s[0], (byte)s[1], (byte)s[2], (byte)s[3] };
        //byte[] bytes2 = { (byte)s[4], (byte)s[5], (byte)s[6], (byte)s[7] };
        //byte[] bytes3 = { (byte)s[8], (byte)s[9], (byte)s[10], (byte)s[11] };
        //
        //return new Vector3(BitConverter.ToSingle(bytes, 0), BitConverter.ToSingle(bytes2, 0), BitConverter.ToSingle(bytes3, 0));

        uint ux = (uint)s[0] + (((uint)s[1]) << 8) + (((uint)s[2]) << 16) + (((uint)s[3]) << 24);
        uint uy = (uint)s[4] + (((uint)s[5]) << 8) + (((uint)s[6]) << 16) + (((uint)s[7]) << 24);
        uint uz = (uint)s[8] + (((uint)s[9]) << 8) + (((uint)s[10]) << 16) + (((uint)s[11]) << 24);
        return new Vector3(UtoFloat(ux), UtoFloat(uy), UtoFloat(uz));
    }
    public static int ToByteInt(string s)
    {
        //byte[] bytes = { (byte)s[0], (byte)s[1], (byte)s[2], (byte)s[3] };
        //return BitConverter.ToInt32(bytes, 0);
        uint u = (uint)s[0] + (((uint)s[1]) << 8) + (((uint)s[2]) << 16) + (((uint)s[3]) << 24);
        return UtoInt(u);
    }
}
