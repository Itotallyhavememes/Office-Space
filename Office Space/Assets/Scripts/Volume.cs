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

     bool changeAudio;
 
    private void Start()
    {
        changeAudio = false;
       

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            Debug.Log("Load was call");
            LoadVolume();
        }
        else
        {
            Debug.Log("Load was not call");
            SetMusicVolume();
            SetFXSVolume();
            SetMusterVolume();
        }

      
        SFXSider.onValueChanged.AddListener((v)=>{
            changeAudio = false;
        });

        MusterSider.onValueChanged.AddListener((v) => {
            changeAudio = false;
        });
        musicSider.onValueChanged.AddListener((v) => {
            changeAudio = false;
        });
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

    private void LoadVolume()
    {
        SFXSider.value = PlayerPrefs.GetFloat("SFXVolume");
        musicSider.value= PlayerPrefs.GetFloat("MusicVolume");
        MusterSider.value = PlayerPrefs.GetFloat("MusterVolume");
        SetMusterVolume();
        SetFXSVolume();
        SetMusicVolume();
    }

    public void ApplyButton()
    {
        Debug.Log("Apply was call");
        SetMusicVolume();
        SetFXSVolume();
        SetMusterVolume();
     
       PlayerPrefs.SetFloat("SFXVolume",SFXSider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSider.value);
        PlayerPrefs.SetFloat("MusterVolume", MusterSider.value);
        changeAudio = true;
    }
    public void myReturn()
    {
        if(!changeAudio)
        {
            Debug.Log("return was call");
            SFXSider.value = PlayerPrefs.GetFloat("SFXVolume");
            musicSider.value = PlayerPrefs.GetFloat("MusicVolume");
            MusterSider.value = PlayerPrefs.GetFloat("MusterVolume");
            SetFXSVolume();
            SetMusterVolume();
            SetMusicVolume();
        }
      
            GameManager.instance.ReturnFromSettings();
    }
}
