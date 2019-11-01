using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ManualUpdater : InitializableObject
{
    public List<Action> ActFirst;
    public List<Action> ActSecond;
    public List<Action> ActThird;
    public List<Action> ActFourth;
    public List<Action> ActFifth;
    public List<Action> ActUpdate;
    public List<Action> ActLateUpdate;
    public List<Action> ActFixedUpdate;

    protected override bool CreateVars()
    {
        FPSManager.FM.Parsed = false;

        if (base.CreateVars())
        {
            ActFirst = new List<Action>();
            ActSecond = new List<Action>();
            ActThird = new List<Action>();
            ActFourth = new List<Action>();
            ActFifth = new List<Action>();
            ActUpdate = new List<Action>();
            ActLateUpdate = new List<Action>();
            ActFixedUpdate = new List<Action>();

            return true;
        }

        return false;
    }

    protected override void DestroyVars()
    {
        ActFirst = null;
        ActSecond = null;
        ActThird = null;
        ActFourth = null;
        ActFifth = null;
        ActUpdate = null;
        ActLateUpdate = null;
        ActFixedUpdate = null;

        base.DestroyVars();
    }

    protected void Update()
    {
        for (int i = ActFirst.Count - 1; i >= 0; --i)
        {
            ActFirst[i]();
        }

        for (int i = ActUpdate.Count - 1; i >= 0; --i)
        {
            ActUpdate[i]();
        }

        for (int i = ActSecond.Count - 1; i >= 0; --i)
        {
            ActSecond[i]();
        }
    }

    protected void LateUpdate()
    {
        for (int i = ActLateUpdate.Count - 1; i >= 0; --i)
        {
            ActLateUpdate[i]();
        }

        for (int i = ActThird.Count - 1; i >= 0; --i)
        {
            ActThird[i]();
        }
    }

    protected void FixedUpdate()
    {
        for (int i = ActFourth.Count - 1; i >= 0; --i)
        {
            ActFourth[i]();
        }

        for (int i = ActFixedUpdate.Count - 1; i >= 0; --i)
        {
            ActFixedUpdate[i]();
        }

        for (int i = ActFifth.Count - 1; i >= 0; --i)
        {
            ActFifth[i]();
        }
    }
}
