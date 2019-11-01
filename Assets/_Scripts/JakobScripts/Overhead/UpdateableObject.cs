using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class UpdateableObject : InitializableObject
{
    bool FirstAdded = false;
    bool SecondAdded = false;
    bool ThirdAdded = false;
    bool FourthAdded = false;
    bool FifthAdded = false;
    bool UpdateAdded = false;
    bool LateUpdateAdded = false;
    bool FixedUpdateAdded = false;

    protected virtual void UpdateObject()
    {

    }

    protected virtual void LateUpdateObject()
    {

    }

    protected virtual void FixedUpdateObject()
    {

    }

    protected virtual void First()
    {

    }

    protected virtual void Second()
    {

    }

    protected virtual void Third()
    {

    }

    protected virtual void Fourth()
    {

    }

    protected virtual void Fifth()
    {

    }

    public void DoUpdate()
    {
        if (EN)
            UpdateObject();
    }

    public void DoLateUpdate()
    {
        if (EN)
            LateUpdateObject();
    }

    public void DoFixedUpdate()
    {
        if (EN)
            FixedUpdateObject();
    }

    public void DoFirst()
    {
        if (EN)
            First();
    }

    public void DoSecond()
    {
        if (EN)
            Second();
    }

    public void DoThird()
    {
        if (EN)
            Third();
    }

    public void DoFourth()
    {
        if (EN)
            Fourth();
    }

    public void DoFifth()
    {
        if (EN)
            Fifth();
    }

    protected void AddFirst()
    {
        if (FPSManager.FM.MU != null)
        {
            FPSManager.FM.MU.ActFirst.Add(DoFirst);
            FirstAdded = true;
        }
    }

    protected void AddSecond()
    {
        if (FPSManager.FM.MU != null)
        {
            FPSManager.FM.MU.ActSecond.Add(DoSecond);
            SecondAdded = true;
        }
    }

    protected void AddThird()
    {
        if (FPSManager.FM.MU != null)
        {
            FPSManager.FM.MU.ActThird.Add(DoThird);
            ThirdAdded = true;
        }
    }

    protected void AddFourth()
    {
        if (FPSManager.FM.MU != null)
        {
            FPSManager.FM.MU.ActFourth.Add(DoFourth);
            FourthAdded = true;
        }
    }

    protected void AddFifth()
    {
        if (FPSManager.FM.MU != null)
        {
            FPSManager.FM.MU.ActFifth.Add(DoFifth);
            FifthAdded = true;
        }
    }

    protected void AddUpdate()
    {
        if (FPSManager.FM.MU != null)
        {
            FPSManager.FM.MU.ActUpdate.Add(DoUpdate);
            UpdateAdded = true;
        }
    }

    protected void AddLateUpdate()
    {
        if (FPSManager.FM.MU != null)
        {
            FPSManager.FM.MU.ActLateUpdate.Add(DoLateUpdate);
            LateUpdateAdded = true;
        }
    }

    protected void AddFixedUpdate()
    {
        if (FPSManager.FM.MU != null)
        {
            FPSManager.FM.MU.ActFixedUpdate.Add(DoFixedUpdate);
            FixedUpdateAdded = true;
        }
    }

    protected override void DestroyVars()
    {
        if (FPSManager.FM.MU != null)
        {
            if (FirstAdded)
                FPSManager.FM.MU.ActFirst.Remove(DoFirst);

            if (SecondAdded)
                FPSManager.FM.MU.ActSecond.Remove(DoSecond);

            if (ThirdAdded)
                FPSManager.FM.MU.ActThird.Remove(DoThird);

            if (FourthAdded)
                FPSManager.FM.MU.ActFourth.Remove(DoFourth);

            if (FifthAdded)
                FPSManager.FM.MU.ActFifth.Remove(DoFifth);

            if (UpdateAdded)
                FPSManager.FM.MU.ActUpdate.Remove(DoUpdate);

            if (LateUpdateAdded)
                FPSManager.FM.MU.ActLateUpdate.Remove(DoLateUpdate);

            if (FixedUpdateAdded)
                FPSManager.FM.MU.ActFixedUpdate.Remove(DoFixedUpdate);
        }

        FirstAdded = false;
        SecondAdded = false;
        ThirdAdded = false;
        FourthAdded = false;
        FifthAdded = false;
        UpdateAdded = false;
        LateUpdateAdded = false;
        FixedUpdateAdded = false;

        base.DestroyVars();
    }
}
