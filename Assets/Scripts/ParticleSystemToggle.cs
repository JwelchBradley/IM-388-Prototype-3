using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ParticleSystemToggle : MonoBehaviour
{
    Rigidbody rb;
    Vector3 velocity;
    ParticleSystem ps;
    [SerializeField]
    float speedLineVelocity = 100;

    // Start is called before the first frame update
    void Start()
    {
        velocity.y = 0;

        if (velocity.magnitude > speedLineVelocity)
        {
            spawnLines();
        }
        else
        {
            despawnLines();
        }
    }

    private void spawnLines()
    {
        ps.Play();
    }

    private void despawnLines()
    {
        ps.Stop();
    }
}
