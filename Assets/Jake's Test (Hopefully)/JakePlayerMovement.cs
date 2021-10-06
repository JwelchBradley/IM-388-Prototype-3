using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JakePlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    public Transform player;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 0.5f;

    Vector3 velocity;
    bool isGrounded;

    // Update is called once per frame
    public void Update()
    {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1f;
        }

        //gets the values of the x and z axis.
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //sets the movement of the player.
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
