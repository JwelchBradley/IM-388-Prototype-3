using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelExit : MonoBehaviour
{



    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            GameObject.Find("Pause Menu Templates Canvas").GetComponent<PauseMenuBehavior>().LoadScene("Main Menu");
            //SceneManager.LoadScene(0);
        }
    }

}
