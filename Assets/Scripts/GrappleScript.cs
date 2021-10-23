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

    private PlayerMovement pm;

    #region Grapple Values
    [Header("Grapple values")]
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

    [Header("Player Assistance")]
    [SerializeField]
    [Tooltip("The size of the box cast that provides slight aim assist")]
    [Range(0, 10)]
    private float aimAssitMultiplier = 5;

    [SerializeField]
    [Tooltip("Controls the size of the sphere")]
    [Range(0, 50)]
    private float sphereSizeMod = 10;

    [Header("Pull Towards")]
    [SerializeField]
    [Tooltip("How fast the player pulls themself towards an object beneath them")]
    [Range(0, 300)]
    private float pullTowardsSpeed = 100;
    #endregion

    public GameObject ropeLeftBehind;

    private bool canGrapple;

    private bool isGrappling;

    private RaycastHit hit;

    private Image crossColor;

    private GameObject grappleLocationSphere;

    public GameObject objectShotAt;

    /// <summary>
    /// Asign variables
    /// </summary>
    public void Start()
    {
        pm = player.GetComponent<PlayerMovement>();
        lineR = GetComponent<LineRenderer>();
        crossColor = inRange.GetComponent<Image>();
    }

    /// <summary>
    /// Checking for input from player
    /// </summary>
    void Update()
    {
        if (pm.NotDead)
        {
            GrappleCheck();

            GrappleWithinRange();

            if (Input.GetMouseButtonDown(0))
            {
                StartGrapple();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                StopGrapple();
            }

            if (Input.GetMouseButtonDown(1))
            {
                PullTowards();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                //StopGrapple();
            }
        }
        else
        {
            canGrapple = false;
            GrappleWithinRange();
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

    #region Grapple Normal
    public GameObject inRange;

    private void GrappleCheck()
    {
        if (!isGrappling)
        {
            canGrapple = Physics.Raycast(cam.position, cam.forward, out hit, maxDist, grappleSurface);

            if (!canGrapple)
                canGrapple = Physics.BoxCast(cam.position, Vector3.one * aimAssitMultiplier, cam.forward, out hit, Quaternion.Euler(45, 0, 0), maxDist, grappleSurface);
        }
    }

    void GrappleWithinRange()
    {
        // Close enough to grapple towards target
        if (canGrapple)
        {
            inRange.transform.localScale = Vector3.one;
            crossColor.color = Color.green;

            if (grappleLocationSphere == null)
            {
                grappleLocationSphere = (GameObject)Instantiate(Resources.Load("Prefabs/Player/Grapple/Sphere", typeof(GameObject)));
            }

            if (isGrappling)
            {
                Destroy(grappleLocationSphere);
            }
            else
            {
                grappleLocationSphere.transform.position = hit.point;
                grappleLocationSphere.transform.localScale = Vector3.one * hit.distance*(sphereSizeMod/100);
            }
            
        }
        // Not close enough
        else
        {
            if (!isGrappling)
            {
                inRange.transform.localScale = Vector3.one / 2;
                crossColor.color = Color.red;
                Destroy(grappleLocationSphere);
            }
        }
    }

    /// <summary>
    /// Creates a line and spring joint
    /// </summary>
    void StartGrapple()
    {
        if (Time.timeScale != 0)
        { 
            if (canGrapple)
            {
                isGrappling = true;

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

                objectShotAt = hit.collider.gameObject;
            }
        }
    }

    /// <summary>
    /// Creates the rope with 2 points
    /// Delete rope if no joint found
    /// </summary>
    void DrawRope()
    {
        //if (!joint) return;
        if (!isGrappling) return;

        lineR.positionCount = 2;
        lineR.SetPosition(0, gunTip.position);
        lineR.SetPosition(1, grapplePoint);

    }

    /// <summary>
    /// Remove Grapple
    /// </summary>
    void StopGrapple()
    {
        isGrappling = false;

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
    #endregion

    #region Grapple Pull Towards
    private void PullTowards()
    {
        if (canGrapple && !isGrappling)
        {
            StartCoroutine(StartPull());
        }
    }

    private IEnumerator StartPull()
    {
        if (Time.timeScale != 0)
        {
            if (canGrapple && hit.point.y < transform.position.y)
            {

                isGrappling = true;

                grapplePoint = hit.point;

                while (Vector3.Distance(hit.point, transform.position)>5)
                {
                    pm.RB.velocity = (hit.point - transform.position).normalized * pullTowardsSpeed;
                    yield return new WaitForFixedUpdate();
                }

                lineR.positionCount = 0;
                isGrappling = false;

                /*
                // Create positions for joint
                grapplePoint = hit.point;
                player.GetComponent<Rigidbody>().AddForce((hit.point - transform.position).normalized*Vector3.Distance(hit.point, transform.position)*10);*/
            }
        }
    }
    #endregion
}
