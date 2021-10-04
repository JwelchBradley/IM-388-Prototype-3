/*****************************************************************************
// File Name :         IgnoreAudioListenerPause.cs
// Author :            Jacob Welch
// Creation Date :     28 August 2021
//
// Brief Description : Does not let this objects audiosource be paused by the
                       AudioListener.
*****************************************************************************/
using UnityEngine;

public class IgnoreAudioListenerPause : MonoBehaviour
{
    /// <summary>
    /// Causes this audiosource to not be paused by the audiolistener.
    /// </summary>
    private void Awake()
    {
        GetComponent<AudioSource>().ignoreListenerPause = true;
    }
}
