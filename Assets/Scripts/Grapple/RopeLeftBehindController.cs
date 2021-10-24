using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLeftBehindController : MonoBehaviour
{
    LineRenderer lineR;
    SpringJoint springJ;

    void Start()
    {
        lineR = GetComponent<LineRenderer>();
        springJ = GetComponent<SpringJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {

        lineR.positionCount = 2;
        lineR.SetPosition(0, transform.position);
        lineR.SetPosition(1, springJ.connectedAnchor);

    }

}
