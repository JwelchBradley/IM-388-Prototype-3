/*****************************************************************************
// File Name :         PlayerMovement.cs
// Author :            Jacob Welch
// Creation Date :     28 August 2021
//
// Brief Description : Handles the movement of the player.
*****************************************************************************/
using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    #region Movement
    [Header("Move Speed")]
    [SerializeField]
    [Tooltip("The acceleration speed while the player is walking")]
    [Range(100, 1000)]
    private float walkForce = 10;

    [SerializeField]
    [Tooltip("The max speed of the player while walking")]
    [Range(0, 20)]
    private float maxWalkVelocity = 10;

    /// <summary>
    /// Holds the current movement speed of the player.
    /// </summary>
    private float currentForce = 10;

    /// <summary>
    /// The max speed the player can currently move at.
    /// </summary>
    private float currentMaxVelocity = 10;

    /// <summary>
    /// Holds the input player movement.
    /// </summary>
    private Vector3 move = Vector3.zero;

    private Rigidbody rb;
    #endregion

    #region Air Movement
    [Header("Jump")]
    [SerializeField]
    [Tooltip("How much velocity the player has when jumping")]
    [Range(100, 10000)]
    private float jumpForce = 300;

    [SerializeField]
    [Tooltip("The rate at which gravity scales")]
    [Range(-200, 0)]
    private float gravity = -9.8f;

    [Header("Grapple")]
    [SerializeField]
    [Tooltip("How fast the player can move while on the grapple")]
    [Range(0, 50)]
    private float grappleMaxVelocity = 20;

    [SerializeField]
    [Tooltip("The amount of control the player has while grappling")]
    [Range(0, 10)]
    private float forwardGrappleMovementMultiplier = 2f;

    [SerializeField]
    [Tooltip("The amount of control the player has while grappling")]
    [Range(0, 10)]
    private float sidewaysGrappleMovementMultiplier = 1.5f;

    [SerializeField]
    private float grappleGravity = -100f;

    /*
    [SerializeField]
    [Tooltip("How much velocity the player has when jumping")]
    [Range(100, 3000)]
    private float crouchJumpForce = 200;
    */

    /// <summary>
    /// The height the player currently jumps to.
    /// </summary>
    private float currentJumpForce = 3;

    /// <summary>
    /// The current gravity being put onto the player.
    /// </summary>
    private float currentGravity = -9.8f;

    [Header("Ground Checks")]
    [SerializeField]
    [Tooltip("How far away the player must be from the ground to be grounded")]
    [Range(0, 2)]
    private float groundCheckDist = 0.5f;

    [SerializeField]
    [Tooltip("Position on the player to check ground from")]
    private GameObject groundCheckPos;

    [SerializeField]
    [Tooltip("The layer mask of the ground")]
    private LayerMask groundMask;

    /// <summary>
    /// Holds true if the player is on the ground.
    /// </summary>
    private bool isGrounded = false;
    #endregion

    #region Cameras
    /// <summary>
    /// The transform of the camera used for movement direction calculations.
    /// </summary>
    private Transform cameraTransform;

    /// <summary>
    /// The virtual camera for when the player is standing.
    /// </summary>
    private CinemachineVirtualCamera walkCam;

    /// <summary>
    /// The CinemachinePOV of the walk camera.
    /// </summary>
    private CinemachinePOV walkCamPOV;

    /// <summary>
    /// The virtual camera for when the player crouching.
    /// </summary>
    private CinemachineVirtualCamera crouchCam;

    /// <summary>
    /// The CinemachinePOV of the crouch camera.
    /// </summary>
    private CinemachinePOV crouchCamPOV;

    public GameObject SludgeOverlay;
    #endregion
    #endregion

    #region Functions
    #region Initilization
    /// <summary>
    /// Initializes the player movement components.
    /// </summary>
    private void Awake()
    {
        currentForce = walkForce;
        currentJumpForce = jumpForce;
        currentMaxVelocity = maxWalkVelocity;
        currentGravity = gravity;

        //controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

        SludgeOverlay.SetActive(false);

        GetCameras();
    }

    #region Cameras
    /// <summary>
    /// Gets all player cameras and components.
    /// </summary>
    private void GetCameras()
    {
        cameraTransform = Camera.main.transform;
        walkCam = GameObject.Find("Walk vcam").GetComponent<CinemachineVirtualCamera>();
        walkCamPOV = walkCam.GetCinemachineComponent<CinemachinePOV>();
        crouchCam = GameObject.Find("Crouch vcam").GetComponent<CinemachineVirtualCamera>();
        crouchCamPOV = crouchCam.GetCinemachineComponent<CinemachinePOV>();

        SetCameraSens();
    }

    private void SetCameraSens()
    {
        walkCamPOV.m_HorizontalAxis.m_MaxSpeed = PlayerPrefs.GetFloat("X Sens");
        walkCamPOV.m_VerticalAxis.m_MaxSpeed = PlayerPrefs.GetFloat("Y Sens");
    }
    #endregion
    #endregion

    #region Input Calls
    /// <summary>
    /// Recieves player input as a Vector2.
    /// </summary>
    /// <param name="move">The Vector2 movement for the player.</param>
    public void MovePlayer(Vector2 move)
    {
        if (notDead)
            this.move = new Vector3(move.x, 0, move.y);
        else
            this.move = Vector3.zero;
    }

    /// <summary>
    /// The player jumps if they are on the ground.
    /// </summary>
    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce);
            isJumping = true;

            Invoke("IsNotJumping", 0.1f);
        }
    }

    private void IsNotJumping()
    {
        isJumping = false;
    }
    bool isJumping = false;
    #endregion

    #region Calculations
    private void FixedUpdate()
    {
        IsGrounded();

        MovePlayer();
    }

    /// <summary>
    /// Checks if the player is on the ground.
    /// </summary>
    private void IsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheckPos.transform.position, groundCheckDist, groundMask);

        if (!isGrounded)
        {
            rb.AddForce(new Vector3(0, currentGravity, 0), ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// Moves the player.
    /// </summary>
    private void MovePlayer()
    {
        SpringJoint sj = GetComponent<SpringJoint>();
        CounterMovement(sj);

        //Some multipliers
        float multiplier = 1f, multiplierZ = 1f;

        // Movement in air
        if (!isGrounded)
        {
            multiplier = 1;
            multiplierZ = 1;
        }

        Vector3 velocity = rb.velocity;
        velocity.y = 0;

        Vector3 currentMove = move;

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        if(sj == null)
        {
            currentGravity = gravity;
        }

        if(sj != null)
        {
            currentGravity = grappleGravity;
            multiplier = forwardGrappleMovementMultiplier;
            multiplierZ = sidewaysGrappleMovementMultiplier;

            if(currentMove.x == 0 || currentMove.z == 0)
            {
                //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
                if (currentMove.x > 0 && xMag > grappleMaxVelocity) currentMove.x = 0;
                if (currentMove.x < 0 && xMag < -grappleMaxVelocity) currentMove.x = 0;

                if (currentMove.z > 0 && yMag > grappleMaxVelocity) currentMove.z = 0;
                if (currentMove.z < 0 && yMag < -grappleMaxVelocity) currentMove.z = 0;
            }
            else
            {
                //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
                if (currentMove.x > 0 && xMag > currentMaxVelocity * .8f) currentMove.x = 0;
                if (currentMove.x < 0 && xMag < -currentMaxVelocity * .8f) currentMove.x = 0;

                if (currentMove.z > 0 && yMag > currentMaxVelocity * .8f) currentMove.z = 0;
                if (currentMove.z < 0 && yMag < -currentMaxVelocity * .8f) currentMove.z = 0;
            }

        }
        else if((currentMove.x == 0 || currentMove.z == 0) && !(currentMove.x == 0 && currentMove.z == 0))
        {
            //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
            if (currentMove.x > 0 && xMag > currentMaxVelocity) currentMove.x = 0;
            if (currentMove.x < 0 && xMag < -currentMaxVelocity) currentMove.x = 0;

            if (currentMove.z > 0 && yMag > currentMaxVelocity) currentMove.z = 0;
            if (currentMove.z < 0 && yMag < -currentMaxVelocity) currentMove.z = 0;
        }
        else if (!isGrounded)
        {
            //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
            if (currentMove.x > 0 && xMag > currentMaxVelocity*.5f) currentMove.x = 0;
            if (currentMove.x < 0 && xMag < -currentMaxVelocity*.5f) currentMove.x = 0;

            if (currentMove.z > 0 && yMag > currentMaxVelocity*.5f) currentMove.z = 0;
            if (currentMove.z < 0 && yMag < -currentMaxVelocity*.5f) currentMove.z = 0;
        }
        else
        {
            if (rb.velocity.magnitude > currentMaxVelocity || rb.velocity.magnitude < -currentMaxVelocity)
            {
                if (currentMove.x > 0 && xMag > currentMaxVelocity/2) currentMove.x = 0;
                if (currentMove.x < 0 && xMag < -currentMaxVelocity/2) currentMove.x = 0;

                if (currentMove.z > 0 && yMag > currentMaxVelocity/2) currentMove.z = 0;
                if (currentMove.z < 0 && yMag < -currentMaxVelocity/2) currentMove.z = 0;
            }
        }

        // Provides less movement when in the air and sliding
        currentMove.x *= multiplier;
        currentMove.z *= multiplier * multiplierZ;

        //Apply forces to move player
        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        Vector3 right = cameraTransform.right;
        right.y = 0;

        rb.AddForce(forward.normalized * currentMove.z * currentForce * multiplier);
        rb.AddForce(right.normalized * currentMove.x * currentForce * multiplierZ);
    }

    private float threshold = 0.01f;
    private float counterMovement = 0.175f;
    private void CounterMovement(SpringJoint sj)
    {
        if (!isGrounded || isJumping || sj != null) return;

        Vector2 mag = FindVelRelativeToLook();

        float x = move.x;
        float y = move.z;

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            Vector3 right = cameraTransform.right;
            right.y = 0;
            rb.AddForce(walkForce * right * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            Vector3 forward = cameraTransform.forward;
            forward.y = 0;
            rb.AddForce(walkForce * forward * -mag.y * counterMovement);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = cameraTransform.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        Vector3 velocity = rb.velocity;
        velocity.y = 0;
        float magnitue = velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }
    #endregion

    #region Collisions
    /*private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Hazard") && notDead)
        {
            notDead = false;
            StartCoroutine(Restart());
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Win") && notDead)
        {
            notDead = false;
            LoadWinScreen();
        }

        if (other.gameObject.CompareTag("Hazard") && notDead)
        {
            notDead = false;
            SludgeOverlay.SetActive(true);
            StartCoroutine(DeathCount(3.0f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Hazard") && !notDead)
        {
            notDead = true;
            SludgeOverlay.SetActive(false);
        }
    }

    private void LoadWinScreen()
    {
        GameObject pauseMenuCanvas = GameObject.Find("Pause Menu Templates Canvas");
        PauseMenuBehavior pmb = pauseMenuCanvas.GetComponent<PauseMenuBehavior>();
        pmb.CanPause = false;

        GameObject winScreen = (GameObject) Instantiate(Resources.Load("Prefabs/Win Screen Variant", typeof(GameObject)));
        winScreen.transform.SetParent(pauseMenuCanvas.transform);
        RectTransform rectTransform = winScreen.GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        Vector3 forward = cameraTransform.forward;
        crouchCam.Follow.transform.forward = forward;
        crouchCam.Priority = 100;

        MenuBehavior mb = winScreen.GetComponent<MenuBehavior>();
        mb.crossfadeAnim = GameObject.Find("Crossfade").GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Confined;
    }

    [Header("Death")]
    [SerializeField]
    private float waitToRestart = 1;
    [SerializeField]
    private AudioClip deathSound;
    private AudioSource aud;
    private Animator anim;
    private bool notDead = true;

    private IEnumerator DeathCount(float waitTime)
    {
        while(!notDead)
        {
            yield return new WaitForFixedUpdate();
            waitTime -= Time.fixedDeltaTime;

            if(waitTime < 0)
            {
                StartCoroutine(Restart());
                break;
            }
        }
    }
    private IEnumerator Restart()
    {
        aud = GetComponent<AudioSource>();
        aud.PlayOneShot(deathSound, 1);
        AudioSource.PlayClipAtPoint(deathSound, transform.position);

        anim = GetComponent<Animator>();
        //anim.SetBool("isDead", true);
        crouchCam.Priority = 100;
        Vector3 forward = cameraTransform.forward;
        crouchCam.Follow.transform.forward = forward;
        forward.x = 0;
        forward.y = 0;
        transform.rotation = cameraTransform.rotation;
        transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
        StartCoroutine(DeathAnim());
        yield return new WaitForSeconds(waitToRestart);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator DeathAnim()
    {
        while (true)
        {
            transform.Rotate(90, 0, 0, Space.Self);
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion
    #endregion
}
