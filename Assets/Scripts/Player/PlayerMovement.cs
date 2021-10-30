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
using UnityEngine.UI;

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

    public Rigidbody RB
    {
        get => rb;
    }
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

    [Header("Exit Grapple")]
    [SerializeField]
    private float exitingGrappleSpeedTime = 1;

    [SerializeField]
    [Tooltip("The max speed player can go to out of grappling")]
    private float maxExitGrappleSpeed = 80;

    [SerializeField]
    private float exitGrappleExtraGravityTime = 2;

    [SerializeField]
    private float exitGrappleGravity = -60;

    private float exitGrappleGravityTime;

    [SerializeField]
    private float fowardExitGrappleMovementMultiplier = 1.2f;

    [SerializeField]
    private float sidewaysExitGrappleMovementMulitplier = 1.5f;

    private float exitGrappleTime;

    private bool exitingGrapple = false;

    [HideInInspector]
    public bool isGrappling = false;

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
    #endregion

    private GameObject sludgeOverlay;
    private Color inTheSauce = new Color(0.4745098f, 0.5849056f, 0.3228017f, 0.4823529f);

    [SerializeField]
    private AudioSource extraAudioSource;

    public AudioSource ExtraAudioSource
    {
        get => extraAudioSource;
    }
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

        sludgeOverlay = GameObject.Find("SludgeOverlay");

        sludgeOverlay.SetActive(false);

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
        if(!PlayerPrefs.HasKey("X Sens"))
        {
            PlayerPrefs.SetFloat("X Sens", 600);
            PlayerPrefs.SetFloat("Y Sens", 400);
        }

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

            if(exitGrappleGravity == currentGravity && rb.velocity.y < -20)
            {
                rb.velocity = new Vector3(rb.velocity.x, -20, rb.velocity.z);
            }
        }
    }

    private float exitGrappleMagnitude = 0;
    /// <summary>
    /// Moves the player.
    /// </summary>
    private void MovePlayer()
    {
        CounterMovement();

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

        if(!isGrappling)
        {
            currentGravity = gravity;
        }

        #region movement limiters
        #region Grappling
        if (isGrappling)
        {
            currentGravity = grappleGravity;
            multiplier = forwardGrappleMovementMultiplier;
            multiplierZ = sidewaysGrappleMovementMultiplier;

            exitGrappleTime = exitingGrappleSpeedTime;
            exitGrappleGravityTime = exitGrappleExtraGravityTime;
            exitingGrapple = true;
            exitGrappleMagnitude = rb.velocity.magnitude;

            if (currentMove.x == 0 || currentMove.z == 0)
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
                if (currentMove.x > 0 && xMag > grappleMaxVelocity * .5f) currentMove.x = 0;
                if (currentMove.x < 0 && xMag < -grappleMaxVelocity * .5f) currentMove.x = 0;

                if (currentMove.z > 0 && yMag > grappleMaxVelocity * .5f) currentMove.z = 0;
                if (currentMove.z < 0 && yMag < -grappleMaxVelocity * .5f) currentMove.z = 0;
            }
        }
        #endregion

        #region exit grapple
        else if (exitingGrapple)
        {
            multiplier = fowardExitGrappleMovementMultiplier;
            multiplierZ = sidewaysExitGrappleMovementMulitplier;
            if (isGrounded || (exitGrappleTime < 0 && exitGrappleGravityTime < 0))
            {
                exitingGrapple = false;
            }
            else
            {
                exitGrappleTime -= Time.fixedDeltaTime;
                exitGrappleGravityTime -= Time.fixedDeltaTime;

                if(exitGrappleMagnitude > maxExitGrappleSpeed)
                {
                    exitGrappleMagnitude = maxExitGrappleSpeed;
                }
                
                if(exitGrappleGravityTime > 0 && rb.velocity.y > 0)
                {
                    currentGravity = exitGrappleGravity;
                }
                else
                {
                    currentGravity = gravity;
                }

                if (currentMove.x == 0 || currentMove.z == 0)
                {
                    //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
                    if (currentMove.x > 0 && xMag > exitGrappleMagnitude) currentMove.x = 0;
                    if (currentMove.x < 0 && xMag < -exitGrappleMagnitude) currentMove.x = 0;

                    if (currentMove.z > 0 && yMag > exitGrappleMagnitude) currentMove.z = 0;
                    if (currentMove.z < 0 && yMag < -exitGrappleMagnitude) currentMove.z = 0;
                }
                else
                {
                    //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
                    if (currentMove.x > 0 && xMag > exitGrappleMagnitude * .5f) currentMove.x = 0;
                    if (currentMove.x < 0 && xMag < -exitGrappleMagnitude * .5f) currentMove.x = 0;

                    if (currentMove.z > 0 && yMag > exitGrappleMagnitude * .5f) currentMove.z = 0;
                    if (currentMove.z < 0 && yMag < -exitGrappleMagnitude * .5f) currentMove.z = 0;
                }
            }
        }
        #endregion

        #region Only One Key
        else if ((currentMove.x == 0 || currentMove.z == 0) && !(currentMove.x == 0 && currentMove.z == 0))
        {
            //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
            if (currentMove.x > 0 && xMag > currentMaxVelocity) currentMove.x = 0;
            if (currentMove.x < 0 && xMag < -currentMaxVelocity) currentMove.x = 0;

            if (currentMove.z > 0 && yMag > currentMaxVelocity) currentMove.z = 0;
            if (currentMove.z < 0 && yMag < -currentMaxVelocity) currentMove.z = 0;
        }
        #endregion

        #region In Air
        else if (!isGrounded)
        {
            //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
            if (currentMove.x > 0 && xMag > currentMaxVelocity*.5f) currentMove.x = 0;
            if (currentMove.x < 0 && xMag < -currentMaxVelocity*.5f) currentMove.x = 0;

            if (currentMove.z > 0 && yMag > currentMaxVelocity*.5f) currentMove.z = 0;
            if (currentMove.z < 0 && yMag < -currentMaxVelocity*.5f) currentMove.z = 0;
        }
        #endregion

        #region Both Keys
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
        #endregion
        #endregion

        // Provides less movement when in the air and sliding
        currentMove.x *= multiplier;
        currentMove.z *= multiplierZ;

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
    private void CounterMovement()
    {
        if (!isGrounded || isJumping || isGrappling) return;

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

        if(rb.velocity.magnitude > maxWalkVelocity)
        {
            rb.velocity *= 0.98f;
        }
    }

    private void LimitGroundVelocity()
    {

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

    bool inAcid = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Win") && notDead)
        {
            notDead = false;
            LoadWinScreen();
        }

        if (other.gameObject.CompareTag("Hazard") && notDead)
        {
            inAcid = true;
            sludgeOverlay.SetActive(true);
            StartCoroutine(DeathCount(3.0f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Hazard"))
        {
            inAcid = false;
            sludgeOverlay.SetActive(false);
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

    public bool NotDead
    {
        get => notDead;
    }

    private IEnumerator DeathCount(float waitTime)
    {

        Image sludgeImage = sludgeOverlay.GetComponent<Image>();
        Color inTheSauce = sludgeImage.color;
        

        while (inAcid)
        {
            yield return new WaitForFixedUpdate();
            waitTime -= Time.fixedDeltaTime;
            inTheSauce.a = 1-(waitTime / 3);
            sludgeImage.color = inTheSauce;

            if (waitTime < 0)
            {
                notDead = false;
                StartCoroutine(Restart());
                break;
            }
        }
    }
    private IEnumerator Restart()
    {
        //aud = GetComponent<AudioSource>();
        extraAudioSource.PlayOneShot(deathSound, 1);
        //AudioSource.PlayClipAtPoint(deathSound, transform.position);

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
