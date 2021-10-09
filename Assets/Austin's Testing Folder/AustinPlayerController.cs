using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AustinPlayerController : MonoBehaviour
{

    //public float pSpeed;
    public GameObject rope;
    public GameObject ropeEnd;
    //public Transform playerDirection;
    public float sizeOfRopePart;

    void Update()
    {
        /* Move
        float xMove = Input.GetAxis("Horizontal");
        float yMove = Input.GetAxis("Vertical");

        if(xMove != 0 || yMove != 0)
        {
            transform.position += new Vector3(xMove * pSpeed * Time.deltaTime, 0, yMove * pSpeed * Time.deltaTime);
        }
        */


        Vector3 direction = Vector3.forward - transform.position;

        // Rope
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ray ropeRay = new Ray(transform.position, direction);
            RaycastHit ropeRayHit;

            if(Physics.Raycast(ropeRay, out ropeRayHit))
            {
                // how far away
                float distance = ropeRayHit.distance;
                print("Distance " + distance);
                float numOfRope = distance / sizeOfRopePart;
                print("num of rope "+ numOfRope);

                // direction
                Vector3 startPoint = transform.position;
                Vector3 endPoint = ropeRayHit.point;
                Vector3 ropeDirection = endPoint - startPoint;
                print("Rope Dir " + ropeDirection);

                // Parent
                

                //create rope
                GameObject pastRope = null;
                float i;
                for (i = 0; i < numOfRope; i += sizeOfRopePart)
                {
                    // Create rope part
                    GameObject currentRope = Instantiate(rope, startPoint, Quaternion.Euler(ropeDirection));

                    // Set new rope creation location
                    Vector3 newStartPoint = currentRope.transform.GetChild(0).position;
                    startPoint = newStartPoint;
                    
                    if (i != 0)
                    {
                        //HingeJoint currentHinge = currentRope.GetComponent<HingeJoint>();
                        HingeJoint pastHinge = pastRope.GetComponent<HingeJoint>();
                        Rigidbody currentRB = currentRope.GetComponent<Rigidbody>();
                        pastHinge.connectedBody = currentRB;
                    }
                    else
                    {
                        currentRope.GetComponent<Rigidbody>().isKinematic = true;
                        currentRope.transform.parent = gameObject.transform;
                        //gameObject.GetComponent<HingeJoint>().connectedBody = currentRope.GetComponent<Rigidbody>();
                    }

                    Debug.DrawRay(currentRope.transform.position, Vector3.forward, Color.blue);

                    pastRope = currentRope;
                }

                Instantiate(ropeEnd, pastRope.transform);

            }


        }

        Debug.DrawRay(transform.position, direction, Color.red);

    }
}
