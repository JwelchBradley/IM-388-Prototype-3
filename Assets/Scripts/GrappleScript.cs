using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GrappleScript : MonoBehaviour
{
    private LineRenderer lineR;
    private Vector3 grapplePoint;
    private SpringJoint joint;

    [Tooltip("Layer player can grapple on.")]
    public LayerMask grappleSurface;

    [Tooltip("Where to shoot grapple from.")]
    public Transform gunTip;

    [Tooltip("Camera and player.")]
    public Transform cam, player;

    [Tooltip("How far player can shoot grapple.")]
    public float maxDist;

    [Tooltip("Max length - use decimal #.")]
    public float maxJointDist;

    [Tooltip("Min length - use decimal #.")]
    public float minJointDist;

    [Tooltip("Spring value of spring joint.")]
    public float springJoint;

    [Tooltip("Damper scale for spring joint.")]
    public float damperJoint;

    [Tooltip("Mass scale for spring joint.")]
    public float massScaleJoint;

    public float toleranceJoint;

    public GameObject ropeLeftBehind;

    /// <summary>
    /// Asign variables
    /// </summary>
    public void Start()
    {
        lineR = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Checking for input from player
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if(Input.GetMouseButtonUp(0))
        { 
            StopGrapple();
        }

        GrappleWithinRange();
    }

    /// <summary>
    /// Causes less jitter... hopefully
    /// </summary>
    private void LateUpdate()
    {
        DrawRope();
    }

    public GameObject inRange;

    void GrappleWithinRange()
    {
        RaycastHit hit;

        Image crossColor = inRange.GetComponent<Image>();

        // Close enough to grapple towards target
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDist))
        {
            inRange.transform.localScale = new Vector3(1f, 1f, 1f);
            crossColor.color = Color.green;
        }
        // Not close enough
        else
        {
            inRange.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            crossColor.color = Color.red;
        }
    }

    /// <summary>
    /// Creates a line and spring joint
    /// </summary>
    void StartGrapple()
    {
        if (Time.timeScale != 0)
        { 

        // Player is within range of a grapple object
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDist))
        {
            // Create positions for joint
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distFromPoint = Vector3.Distance(player.position, grapplePoint);

            // Joint settings
            joint.maxDistance = distFromPoint * maxJointDist;
            joint.minDistance = distFromPoint * minJointDist;

            joint.spring = springJoint;
            joint.damper = damperJoint;
            joint.massScale = massScaleJoint;
            joint.tolerance = toleranceJoint;
            }
        }
    }

    /// <summary>
    /// Creates the rope with 2 points
    /// Delete rope if no joint found
    /// </summary>
    void DrawRope()
    {
        if (!joint) return;

        lineR.positionCount = 2;
        lineR.SetPosition(0, gunTip.position);
        lineR.SetPosition(1, grapplePoint);

    }

    /// <summary>
    /// Remove Grapple
    /// </summary>
    void StopGrapple()
    {
        if (ropeLeftBehind != null)
        {
            // Leave rope behind
            SpringJoint sj = ropeLeftBehind.GetComponent<SpringJoint>();
            sj.autoConfigureConnectedAnchor = false;
            sj.connectedAnchor = grapplePoint;

            float distFromPoint = Vector3.Distance(player.position, grapplePoint);

            sj.maxDistance = distFromPoint * maxJointDist;
            sj.minDistance = distFromPoint * minJointDist;

            sj.spring = springJoint;
            sj.damper = damperJoint;
            sj.massScale = massScaleJoint;

            Instantiate(ropeLeftBehind, transform.position, transform.rotation);
            //
        }

        lineR.positionCount = 0;
        Destroy(joint);
    }

}
