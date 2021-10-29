using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RingPoints : MonoBehaviour
{
    MeshRenderer[] mr;
    //public Material green;

    public ParticleSystem part;

    public GameObject doors;
    bool oneTime = false;

    public AudioClip ringSnd;
    public Transform player;

    private AudioSource aud;

    private bool hasTriggered = false;

    private void Awake()
    {
        aud = GetComponent<AudioSource>();
        mr = GetComponentsInChildren<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !hasTriggered)
        {

            if (!oneTime)
            {
                doors.GetComponent<DoorControl>().currentUnLocks += 1;
            }

            //mr = GetComponent<MeshRenderer>();
            //mr.material = green;

            oneTime = true;
            hasTriggered = true;

            Vector3 pos = gameObject.transform.position;

            aud.PlayOneShot(ringSnd, 1);
            //AudioSource.PlayClipAtPoint(ringSnd, player.position);
            foreach(MeshRenderer meshr in mr)
            {
                meshr.enabled = false;
            }
            Instantiate(part, pos, transform.rotation);
            Destroy(gameObject, 3);

        }
    }

}
