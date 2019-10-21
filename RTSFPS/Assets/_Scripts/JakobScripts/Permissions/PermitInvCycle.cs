using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermitInvCycle : AbstractPermission<PermitInvCycle, CommandInvCycle, Inventory, Controller>//AbsInvPermission<PermitInvCycle, CommandInvCycle, Inventory, Controller>
{
    public float timeDelay = 0f;
    float timeRem = 0.2f;

    public void ReceiveInput(int axisValue)
    {
        if (axisValue != 0 && timeRem <= 0f && SpecificActor.Items.Amount > 0)
        {
            int newVal = axisValue + SpecificActor.activeObject;
            while (newVal < 0)
            {
                newVal += SpecificActor.Items.Amount;
            }
            while (newVal >= SpecificActor.Items.Amount)
            {
                newVal -= SpecificActor.Items.Amount;
            }

            SpecificActor.SwapActive(newVal);

            timeRem = timeDelay;
        }
    }

    private void Update()
    {
        if (timeRem > 0f)
        {
            timeRem -= Time.deltaTime;
        }
    }

    protected override void FeedPuppet()
    {

    }
}
