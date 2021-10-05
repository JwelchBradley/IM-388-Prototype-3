using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AustinMovementTest : MonoBehaviour
{

    public float mouseSpeed;
    public Transform playerBody;
    public Rigidbody playerRB;
    float xRot = 0f;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);




        float xMove = Input.GetAxis("Horizontal");
        float zMove = Input.GetAxis("Vertical");

        if(xMove != 0 || zMove != 0)
        {
            playerRB.AddForce(xMove * 50, 0, zMove * 50);

            //playerBody.transform.forward = transform.eulerAngles;
            //playerBody.transform.position += new Vector3(xMove, 0, zMove);
        }

        //Vector3 move = transform.right * xMove + transform.forward * zMove;



    }
}
