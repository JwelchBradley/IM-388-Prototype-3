/*****************************************************************************
// File Name :         MenuBehavior.cs
// Author :            Jacob Welch
// Creation Date :     28 August 2021
//
// Brief Description : Handles the chaning of panels and scenes from menus.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour
{
    #region Variables
    //[SerializeField]
    [Tooltip("Animator of level crossfade")]
    public Animator crossfadeAnim;

    [SerializeField]
    [Tooltip("Tick true if there is a loading screen")]
    protected bool hasLoadScreen;

    [Header("Panels")]
    [Tooltip("The panels for the settings screen")]
    [SerializeField]
    private List<GameObject> settingsScreens = new List<GameObject>();
    #endregion

    #region Functions
    /// <summary>
    /// Sets a panel to be active.
    /// </summary>
    /// <param name="panel">The panel to be set active.</param>
    public void LoadPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    /// <summary>
    /// Sets a panel to be inactive.
    /// </summary>
    /// <param name="panel">The panel to be set inactive.</param>
    public void DisablePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// Loads a scene from a given string.
    /// </summary>
    /// <param name="sceneName">Scene to be loaded.</param>
    public void LoadScene(string sceneName)
    {
        if (!hasLoadScreen)
        {
            StartCoroutine(LoadSceneHelper(sceneName));
        }
    }

    public void PlayAudio(AudioSource audio)
    {
        audio.PlayOneShot(audio.clip);
    }

    public void DisableOtherSettingsPanels(GameObject newPanel)
    {
        newPanel.SetActive(true);

        foreach(GameObject panel in settingsScreens)
        {
            if(panel != newPanel)
            {
                panel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Loads a scene after the level has faded out.
    /// </summary>
    /// <param name="sceneName">Scene to be loaded.</param>
    /// <returns></returns>
    protected IEnumerator LoadSceneHelper(string sceneName)
    {
        crossfadeAnim.SetBool("levelEnd", true);

        yield return null;

        AsyncOperation loadOp;

        if (sceneName.Equals("Next Level"))
        {
            loadOp = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex+1);
        }
        else
        {
            loadOp = SceneManager.LoadSceneAsync(sceneName);
        }
        // Preloads the scene and then loads it after the scene has faded out
        
        loadOp.allowSceneActivation = false;

        yield return new WaitForSecondsRealtime(crossfadeAnim.GetCurrentAnimatorStateInfo(0).length);

        // Resets game
        Time.timeScale = 1;
        AudioListener.pause = false;

        loadOp.allowSceneActivation = true;
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
    #endregion
}