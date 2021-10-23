/*****************************************************************************
// File Name :         SliderBehavior.cs
// Author :            Jacob Welch
// Creation Date :     15 June 2021
//
// Brief Description : Handles the sliders in the opitons menu.
*****************************************************************************/
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderBehavior : MonoBehaviour
{
    #region Variables
    #region Defaults
    [Header("Defaults")]
    [SerializeField]
    [Tooltip("The variable name used by player prefs")]
    private string variableName;

    [SerializeField]
    [Tooltip("This slider object")]
    private Slider slider;

    public Slider M_Slider
    {
        get => slider;
    }

    [SerializeField]
    [Tooltip("The input field for this slider")]
    private TMP_InputField inputField;
    #endregion

    #region Sensitivity
    [Header("Sensitivity")]
    [SerializeField]
    [Tooltip("True if this is for horizontal sensitivity")]
    private bool isSens;

    /// <summary>
    /// The CinemachinePOV of the walk camera.
    /// </summary>
    private CinemachinePOV walkCamPOV;
    #endregion

    #region Volume
    [Header("Volume")]
    [SerializeField]
    [Tooltip("Set true if this slider controls volume")]
    private bool isVolume;

    [SerializeField]
    [Tooltip("The audio mixer that is to be modified")]
    private AudioMixer audioMixer;
    #endregion
    #endregion

    #region Initialize
    /// <summary>
    /// Before the first frame is sets the slider or creates a playerpref.
    /// </summary>
    void Awake()
    {
        InitializePlayerPrefs();

        if (isVolume)
        {
            SetVolume(PlayerPrefs.GetFloat(variableName));
        }        
        else if(isSens)
        {
            GameObject vcam = GameObject.Find("Walk vcam");
            
            if(vcam != null)
            {
                walkCamPOV = vcam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>();
            }
        }
    }

    private void InitializePlayerPrefs()
    {
        if (PlayerPrefs.HasKey(variableName))
        {
            slider.value = PlayerPrefs.GetFloat(variableName);
        }
        else
        {
            if (isVolume)
            {
                PlayerPrefs.SetFloat(variableName, 1);
            }
            else if (isSens)
            {
                if (walkCamPOV != null)
                {
                    switch (variableName)
                    {
                        case "X Sens":
                            PlayerPrefs.SetFloat(variableName, 600);
                            break;
                        case "Y Sens":
                            PlayerPrefs.SetFloat(variableName, 400);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
    #endregion

    public void SetSetting(float sliderValue)
    {
        if (isVolume)
        {
            SetVolume(sliderValue);
        }
        else if (isSens)
        {
            SetSensitivity(sliderValue);
        }
    }

    private void SetSensitivity(float sliderValue)
    {
        PlayerPrefs.SetFloat(variableName, sliderValue);

        SetInputField();

        if(walkCamPOV != null)
        {
            switch(variableName)
            {
                case "X Sens":
                    walkCamPOV.m_HorizontalAxis.m_MaxSpeed = sliderValue;
                        break;
                case "Y Sens":
                    walkCamPOV.m_VerticalAxis.m_MaxSpeed = sliderValue;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Sets the volume of of the audiomixer and playerpref.
    /// </summary>
    /// <param name="sliderValue">The value from the slider.</param>
    private void SetVolume(float sliderValue)
    {
        // Converts linear slider value to exponential Audio Group value
        float vol = Mathf.Log10(sliderValue) * 20;

        audioMixer.SetFloat(variableName, vol);

        // Saves player audio adjustment
        PlayerPrefs.SetFloat(variableName, slider.value);

        SetInputField();
    }

    private void SetInputField()
    {
        if(inputField != null)
        {
            float value = PlayerPrefs.GetFloat(variableName);

            if (value == 0.001f)
            {
                value = 0;
            }

            inputField.text = value.ToString();
        }
    }
}
