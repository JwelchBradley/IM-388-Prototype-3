using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorControl : MonoBehaviour
{


    public int unlockAmount;
    public int currentUnLocks = 0;

    public GameObject leftDoor;
    public GameObject rightDoor;

    public GameObject leftDoorOpen;
    public GameObject rightDoorOpen;


    public float doorOpenSpeed;
    bool oneTime = true;

    public Text objective;


    void Update()
    {
        objective.text = ("Rings: " + currentUnLocks + " / " + unlockAmount);

        if(currentUnLocks >= unlockAmount && oneTime)
        {
            DoorOpen();
        }
    }


    void DoorOpen()
    {

        leftDoor.transform.position = Vector3.MoveTowards(leftDoor.transform.position, leftDoorOpen.transform.position, doorOpenSpeed);
        rightDoor.transform.position = Vector3.MoveTowards(rightDoor.transform.position, rightDoorOpen.transform.position, doorOpenSpeed);

        if (Vector3.Distance(leftDoor.transform.position, leftDoorOpen.transform.position) <= 0.01f)
        {
            oneTime = false;
        }
        
    }
}
