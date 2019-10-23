using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class MapLoader : MonoBehaviour
{
    private static MapLoader _instance;
    public static MapLoader Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    const string DLL_NAME = "MapLoader";
    [DllImport(DLL_NAME)]
    public static extern void saveItem(int t, float x, float y, float z);

    [DllImport(DLL_NAME)]
    public static extern void loadMap();

    [DllImport(DLL_NAME)]
    public static extern void clearFile();

    [DllImport(DLL_NAME)]
    public static extern int getObjectAmount();

    [DllImport(DLL_NAME)]
    public static extern int getType(int id);

    [DllImport(DLL_NAME)]
    public static extern float getX(int id);

    [DllImport(DLL_NAME)]
    public static extern float getY(int id);

    [DllImport(DLL_NAME)]
    public static extern float getZ(int id);

}
