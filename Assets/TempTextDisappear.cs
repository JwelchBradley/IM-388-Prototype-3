using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TempTextDisappear : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Invoke("ChangeText", 4);
    }

    private void ChangeText()
    {
        GetComponent<TextMeshProUGUI>().text = "Pause and go to main menu when finished";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
