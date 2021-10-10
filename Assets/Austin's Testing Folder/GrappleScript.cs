using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    /// <summary>
    /// Causes less jitter... hopefully
    /// </summary>
    private void LateUpdate()
    {
        DrawRope();
    }

    /// <summary>
    /// Creates a line and spring joint
    /// </summary>
    void StartGrapple()
    {
        // Player is within range of a grapple object
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxDist))
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
        lineR.positionCount = 0;
        Destroy(joint);
    }

}
