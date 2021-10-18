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
    float speedLineVelocity = 10;

    // Start is called before the first frame update
    void FixedUpdate()
    {
        velocity = rb.velocity;

        velocity.y = 0;

        Awake();

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

    private void Awake()
    {
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();
        ps = GetComponent<ParticleSystem>();

    }
}