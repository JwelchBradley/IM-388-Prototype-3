/*****************************************************************************
// File Name :         PanelDisabler.cs
// Author :            Jacob Welch
// Creation Date :     28 August 2021
//
// Brief Description : Disables this panel if the player presses escape.
*****************************************************************************/
using UnityEngine;

public class PanelDisabler : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }
}
