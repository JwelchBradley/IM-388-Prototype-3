using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RingPoints : MonoBehaviour
{
    MeshRenderer mr;
    //public Material green;

    public ParticleSystem part;

    public GameObject doors;
    bool oneTime = false;

    public AudioClip ringSnd;
    public Transform player;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {

            if (!oneTime)
            {
                doors.GetComponent<DoorControl>().currentUnLocks += 1;
            }

            //mr = GetComponent<MeshRenderer>();
            //mr.material = green;

            oneTime = true;

            Vector3 pos = gameObject.transform.position;

            AudioSource.PlayClipAtPoint(ringSnd, player.position);
            Instantiate(part, pos, transform.rotation);
            GameObject.Destroy(gameObject);

        }
    }

}
