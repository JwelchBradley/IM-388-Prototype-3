using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeUse : MonoBehaviour
{
    GrappleScript gs;
    public GameObject player;
    public Material red;
    // Start is called before the first frame update
    void Start()
    {
        gs = player.GetComponent<GrappleScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gs.objectShotAt == gameObject && Input.GetMouseButtonUp(0))
        {
            gameObject.layer = 0;
            
        }
    }


}
