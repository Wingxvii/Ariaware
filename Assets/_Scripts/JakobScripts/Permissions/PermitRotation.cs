using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PermitRotation : AbstractPermission<PermitRotation, CommandRotation, Puppet, Controller>
{
    Vector3 storedRotation = Vector3.zero;
    public Vector3 multiplier = Vector3.one;

    public AngleClamp xConstraint = AngleClamp.Immobile;
    public AngleClamp yConstraint = AngleClamp.Immobile;
    public AngleClamp zConstraint = AngleClamp.Immobile;

    public void AddRotation(Vector3 rot)
    {
        for (int i = 0; i < 3; i++)
            storedRotation[i] += multiplier[i] * rot[i];
    }

    void AddSingular(ref float newRot, AngleClamp axis, int axisNum)
    {
        if (axis == AngleClamp.Immobile)
            return;
        newRot += storedRotation[axisNum];
        while (newRot > 180f)
            newRot -= 360f;
        while (newRot <= -180f)
            newRot += 360f;
        if (axis == AngleClamp.NinetyNinety)
            newRot = Mathf.Clamp(newRot, -90f, 90f);
    }

    protected override void FeedPuppet()
    {
        Vector3 eulers = SpecificActor.transform.localRotation.eulerAngles;

        AddSingular(ref eulers.x, xConstraint, 0);
        AddSingular(ref eulers.y, yConstraint, 1);
        AddSingular(ref eulers.z, zConstraint, 2);

        SpecificActor.transform.localRotation = Quaternion.Euler(eulers);

        storedRotation = Vector3.zero;
    }

    public enum AngleClamp
    {
        None,
        Immobile,
        NinetyNinety
    }
}
