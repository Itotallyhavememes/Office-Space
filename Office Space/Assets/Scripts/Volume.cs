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

    // bool changeAudio;


    private void Awake()
    {
      
        musicSider.onValueChanged.AddListener(HandleSider);
        MusterSider.onValueChanged.AddListener (SilderMaster);
        SFXSider.onValueChanged.AddListener(SilderSFX);
        //DebugLog("Awake");
    }


    private void OnDisable()
    {
        PlayerPrefs.SetFloat("music", musicSider.value);
        PlayerPrefs.SetFloat("Master", MusterSider.value);
        PlayerPrefs.SetFloat("SFX", SFXSider.value);
    }
   
    private void HandleSider(float value)
    {
        if (musicSider.value == 0)
            audioMixer.SetFloat("music", -80f);
        else
            audioMixer.SetFloat("music", Mathf.Log10(value) * 30f);
    }
    private void SilderSFX(float value)
    {
        if (SFXSider.value == 0)
            audioMixer.SetFloat("SFX", -80f);
        else
            audioMixer.SetFloat("SFX", Mathf.Log10(value) * 30f);
    }
    private void SilderMaster(float value)
    {
        if (MusterSider.value == 0)
            audioMixer.SetFloat("Master", -80f);
        else
            audioMixer.SetFloat("Master", Mathf.Log10(value) * 30f);
    }
    private void Start()
    {

       // changeAudio = false;
        //DebugLog("Start");
      musicSider.value = PlayerPrefs.GetFloat("music", musicSider.value);
        SFXSider.value = PlayerPrefs.GetFloat("SFX", SFXSider.value);
        MusterSider.value = PlayerPrefs.GetFloat("Master", MusterSider.value);
        //SFXSider.onValueChanged.AddListener((v) =>
        //{
        //    changeAudio = false;
        //});

        //MusterSider.onValueChanged.AddListener((v) =>
        //{
        //    changeAudio = false;
        //});
        //musicSider.onValueChanged.AddListener((v) =>
        //{
        //    changeAudio = false;
        //});
    }



    //public void SetMusterVolume()
    //{
    //    //DebugLog("Chnage in Master");
    //    float volume = MusterSider.value;
    //    audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);

    //}
    //public void SetMusicVolume()
    //{
    //    float volume = musicSider.value;
    //    audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);

    //}
    //public void SetFXSVolume()
    //{
    //    float volume = SFXSider.value;
    //    audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);

    //}



    //public void ApplyButton()
    //{
    //    //DebugLog("Apply was call");
    //    SetMusicVolume();
    //    SetFXSVolume();
    //    SetMusterVolume();

    //   PlayerPrefs.SetFloat("SFXVolume",SFXSider.value);
    //    PlayerPrefs.SetFloat("MusicVolume", musicSider.value);
    //    PlayerPrefs.SetFloat("MusterVolume", MusterSider.value);
    //    changeAudio = true;
    //}
    //public void myReturn()
    //{
    //    if(!changeAudio)
    //    {
    //        //DebugLog("return was call");
         //   SFXSider.value = PlayerPrefs.GetFloat("SFXVolume");
           // musicSider.value = PlayerPrefs.GetFloat("MusicVolume");
         //   MusterSider.value = PlayerPrefs.GetFloat("MusterVolume");
          //  SetFXSVolume();
         //   SetMusterVolume();
          //  SetMusicVolume();
    //    }
      
    //        GameManager.instance.ReturnFromSettings();
    //}
}
