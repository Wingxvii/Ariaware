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

    public bool netParsed = false;
    NET_PACKET.NetworkDataManager ndm;
    public NET_PACKET.NetworkDataManager NDM
    {
        get { if (ndm == null && !netParsed) { ndm = Object.FindObjectOfType<NET_PACKET.NetworkDataManager>(); } netParsed = true; return ndm; }
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
