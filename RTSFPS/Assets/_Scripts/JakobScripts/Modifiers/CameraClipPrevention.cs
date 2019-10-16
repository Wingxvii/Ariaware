using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraAnchor))]
public class CameraClipPrevention : Modifier
{
    public bool EnableClipPrevention = true;

    CameraAnchor anchor;
    public CameraAnchor Anchor
    {
        get { return anchor; }
        protected set { anchor = value; }
    }

    Vector3 priorPosition = Vector3.zero;

    protected override bool CreateVars()
    {
        if (base.CreateVars())
        {
            Anchor = GetComponent<CameraAnchor>();

            return false;
        }

        return true;
    }

    protected override void DestroyVars()
    {
        Anchor = null;

        base.DestroyVars();
    }

    private void OnPreRender()
    {
        if (EnableClipPrevention)
            ApplyPreRenderModifier();
    }

    public void ApplyPreRenderModifier()
    {
        priorPosition = transform.localPosition;
        Transform tform = transform.parent;
        if (tform != null && priorPosition.magnitude > 0)
        {
            float ppMag = priorPosition.magnitude;
            RaycastHit[] hit;

            hit = Physics.RaycastAll(tform.position, tform.rotation * priorPosition / ppMag, ppMag);
            int closest = -1;

            Body bod = Anchor.LocalBody.GetObj(0);

            for (int i = 0; i < hit.Length; i++)
            {
                //Body bhit = hit[i].collider.GetComponent<Body>();
                bool cancelHit = false;
                if (bod != null)
                {
                    for (int j = bod.Col.Length - 1; j >= 0; --j)
                    {
                        if (bod.Col[j] == hit[i].collider)
                        {
                            cancelHit = true;
                            j = -1;
                        }
                    }
                }

                if (!cancelHit)
                {
                    if (closest < 0)
                        closest = i;
                    else if (hit[closest].distance > hit[i].distance)
                        closest = i;
                }
                
            }

            if (closest >= 0)
            {
                transform.localPosition = Vector3.ClampMagnitude(transform.localPosition, hit[closest].distance);
            }
        }
    }

    private void OnPostRender()
    {
        if (EnableClipPrevention)
            ApplyPostRenderModifier();
    }

    public void ApplyPostRenderModifier()
    {
        transform.localPosition = priorPosition;
    }
}
