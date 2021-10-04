/*****************************************************************************
// File Name :         SliderBehavior.cs
// Author :            Jacob Welch
// Creation Date :     15 June 2021
//
// Brief Description : Handles the sliders in the opitons menu.
*****************************************************************************/
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderBehaviour : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The variable name used by the audio mixer")]
    private string volVariableName;

    [SerializeField]
    [Tooltip("This slider object")]
    private Slider volSlider;

    [SerializeField]
    [Tooltip("The audio mixer that is to be modified")]
    private AudioMixer audioMixer;

    /// <summary>
    /// Before the first frame is sets the slider or creates a playerpref.
    /// </summary>
    void Start()
    {
        if (PlayerPrefs.HasKey(volVariableName))
        {
            volSlider.value = PlayerPrefs.GetFloat(volVariableName);
        }
        else
        {
            PlayerPrefs.SetFloat(volVariableName, 10);
        }

        SetVolume(PlayerPrefs.GetFloat(volVariableName));
    }

    /// <summary>
    /// Sets the volume of of the audiomixer and playerpref.
    /// </summary>
    /// <param name="sliderValue">The value from the slider.</param>
    public void SetVolume(float sliderValue)
    {
        // Converts linear slider value to exponential Audio Group value
        float vol = Mathf.Log10(sliderValue) * 20;

        audioMixer.SetFloat(volVariableName, vol);

        // Saves player audio adjustment
        PlayerPrefs.SetFloat(volVariableName, volSlider.value);
    }
}
