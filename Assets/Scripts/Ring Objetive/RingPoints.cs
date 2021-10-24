using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RingPoints : MonoBehaviour
{
    MeshRenderer mr;
    public Material green;

    public GameObject doors;
    bool oneTime = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {

            if (!oneTime)
            {
                doors.GetComponent<DoorControl>().currentUnLocks += 1;
            }

            mr = GetComponent<MeshRenderer>();
            mr.material = green;

            oneTime = true;

        }
    }

}
