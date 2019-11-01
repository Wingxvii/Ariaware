using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSManager
{
    public bool Parsed = false;

    ManualUpdater mU;
    public ManualUpdater MU
    {
        get { if (mU == null && !Parsed) { mU = Object.FindObjectOfType<ManualUpdater>(); } Parsed = true; return mU; }
    }

    static FPSManager fM;
    public static FPSManager FM
    {
        get { if (fM == null) { fM = new FPSManager(); } return fM; }
    }

    private FPSManager()
    {

    }


}
