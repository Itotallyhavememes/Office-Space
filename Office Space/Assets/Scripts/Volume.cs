using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider MusterSider;
    [SerializeField] private Slider musicSider;
    [SerializeField] private Slider SFXSider;
    private void Start()
    {
        SetMusterVolume();
        SetMusicVolume();
        SetFXSVolume();
    }

    public void SetMusterVolume()
    {
        float volume = MusterSider.value;
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);


    }
    public void SetMusicVolume()
    {
        float volume = musicSider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);


    }
    public void SetFXSVolume()
    {
        float volume = SFXSider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);


    }
}
