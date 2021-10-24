using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    private LineRenderer lineR;
    private GrappleScript gs;
    private Spring spring;
    private Vector3 currentGrapplePosition;

    [SerializeField]
    private int quality = 10;

    [SerializeField]
    private float strength;
    [SerializeField]
    private float damper;
    [SerializeField]
    private float velocity;
    [SerializeField]
    private float waveHeight;
    [SerializeField]
    private float waveCount;
    [SerializeField]
    private AnimationCurve affectCurve;

    /// <summary>
    /// Asign variables
    /// </summary>
    public void Awake()
    {
        lineR = GetComponent<LineRenderer>();
        gs = GetComponent<GrappleScript>();
        spring = new Spring();
        spring.SetTarget(0);
    }

    /// <summary>
    /// Causes less jitter... hopefully
    /// </summary>
    private void LateUpdate()
    {
        DrawRope();
    }

    /// <summary>
    /// Creates the rope with 2 points
    /// Delete rope if no joint found
    /// </summary>
    void DrawRope()
    {
        //if (!joint) return;
        if (!gs.IsGrappling)
        {
            currentGrapplePosition = gs.gunTip.position;
            spring.Reset();
            if(lineR.positionCount > 0)
            {
                lineR.positionCount = 0;
            }
            return;
        }

        if(lineR.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lineR.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        var grapplePoint = gs.GrapplePoint;
        var gunTipPosition = gs.gunTip.position;
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime*12f);

        for(int i = 0; i < quality+1; i++)
        {
            var delta = i/(float) quality;
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI * spring.Value * affectCurve.Evaluate(delta));

            lineR.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
        }



        /*
        lineR.positionCount = 2;
        lineR.SetPosition(0, gs.gunTip.position);
        lineR.SetPosition(1, gs.GrapplePoint);*/

    }
}
