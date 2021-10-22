using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RingPoints : MonoBehaviour
{

    public Text score;

    private float points = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            points += 1f;
            score.text = "Points: " + points;
        }
    }

}
