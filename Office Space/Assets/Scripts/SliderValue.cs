using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SliderValue : MonoBehaviour
{
    [SerializeField] TMP_Text sliderValue;

    public void updateTimerSliderValue(float value)
    {
        sliderValue.text = value.ToString() + ":00";
        GameManager.instance.SetDKTimer((int)value * 60); //Timer format is in seconds
    }
}
