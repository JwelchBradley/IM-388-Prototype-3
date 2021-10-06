using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleScript : MonoBehaviour
{
    private LineRenderer lineR;
    private Vector3 grapplePoint;
    public LayerMask grappleSurface;
    public Transform gunTip;
    public Transform cam, player;
    public float maxDist;
    private SpringJoint joint;

    public void Start()
    {
        lineR = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
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

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxDist))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distFromPoint * 0.8f;
            joint.minDistance = distFromPoint * 0.7f;

            joint.spring = 4.5f;
            joint.damper = 2f;
            joint.massScale = 4.5f;

        }
    }

    void DrawRope()
    {
        if (!joint) return;

        lineR.positionCount = 2;
        lineR.SetPosition(0, gunTip.position);
        lineR.SetPosition(1, grapplePoint);

    }

    void StopGrapple()
    {
        lineR.positionCount = 0;
        Destroy(joint);
    }

}
