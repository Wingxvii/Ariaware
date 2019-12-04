using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace dll
{
    public class UserMetrics
    {
        const string DLL_NAME = "UserMetrics";
        [DllImport(DLL_NAME)]
        public static extern void UpdateFile();
        [DllImport(DLL_NAME)]
        public static extern void ClearFile();
        [DllImport(DLL_NAME)]
        public static extern void BuildingIncrease();
        [DllImport(DLL_NAME)]
        public static extern void TurretIncrease();
        [DllImport(DLL_NAME)]
        public static extern void DroidIncrease();
        [DllImport(DLL_NAME)]
        public static extern void CreditEarnedIncrease(int amount);
        [DllImport(DLL_NAME)]
        public static extern void CreditSpentIncrease(int amount);
    }
}
