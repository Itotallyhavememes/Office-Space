using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class SliderValue : MonoBehaviour
{
    [SerializeField] TMP_Text sliderValue;

    public void updateTimerSliderValue(float value)
    {
        sliderValue.text = value.ToString() + ":00";
        GameManager.instance.SetDKTimer((int)value * 60); //Timer format is in seconds
    }

    public void updateMouseSensitivity(float value)
    {
        foreach (var player in PlayerManager.instance.players)
        {
            if (player.GetComponent<ControllerTest>().multEventSystem.playerRoot == GameManager.instance.globalUI)
                player.GetComponent<ControllerTest>().SetMouseSensitivity(value);
        }
    }
    
    public void updateControllerSensitivity(float value)
    {
        foreach (var player in PlayerManager.instance.players)
        {
            if (player.GetComponent<ControllerTest>().multEventSystem.playerRoot == GameManager.instance.globalUI)
                player.GetComponent<ControllerTest>().SetControllerSensitivity(value);            
        }
    }
}
