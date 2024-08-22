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



    private void Awake()
    {
        musicSider.value = 10f;
        MusterSider.value = 10f;
        SFXSider.value = 10f;
        musicSider.onValueChanged.AddListener(HandleSider);
        MusterSider.onValueChanged.AddListener (SilderMaster);
        SFXSider.onValueChanged.AddListener(SilderSFX);
        Debug.Log("Awake");
    }


    private void OnDisable()
    {
        PlayerPrefs.SetFloat("music", -80f);
        PlayerPrefs.SetFloat("Master", -80f);
        PlayerPrefs.SetFloat("SFX", -80f);
    }
    private void HandleSider(float value)
    {
        if (musicSider.value == 0)
            audioMixer.SetFloat("music", -80f);
        else
            audioMixer.SetFloat("music", Mathf.Log10(value) * 10f);
    }
    private void SilderSFX(float value)
    {
        if (SFXSider.value == 0)
            audioMixer.SetFloat("SFX", -80f);
        else
            audioMixer.SetFloat("SFX", Mathf.Log10(value) * 10f);
    }
    private void SilderMaster(float value)
    {
        if (MusterSider.value == 0)
            audioMixer.SetFloat("Master", -80f);
        else
            audioMixer.SetFloat("Master", Mathf.Log10(value) * 10f);
    }
    private void Start()
    {

       // changeAudio = false;
        Debug.Log("Start");


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
    //    Debug.Log("Chnage in Master");
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
    //    Debug.Log("Apply was call");
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
    //        Debug.Log("return was call");
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
