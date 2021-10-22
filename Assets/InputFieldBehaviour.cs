using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldBehaviour : MonoBehaviour
{
    [Header("Defaults")]
    [SerializeField]
    [Tooltip("The slider behaviour of the slider next to this object")]
    private SliderBehavior sb;

    [SerializeField]
    private bool isVolume;

    public void SetSlider(string inputString)
    {
        if(inputString != "" && inputString != "0.001")
        {
            float value = float.Parse(inputString);
            SetSliderHelper(value);
        }
        else if(inputString != "0.001")
        {
            SetSliderHelper(0);
        }
    }

    private void SetSliderHelper(float value)
    {
        if(isVolume && value <= 0.001f)
        {
            GetComponent<TMP_InputField>().text = 0.ToString();
        }
        else
        {
            sb.SetSetting(value);

            sb.M_Slider.value = value;
        }
    }
}
