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
        musicSider.onValueChanged.AddListener(HandleSider);
        MusterSider.onValueChanged.AddListener (SilderMaster);
        SFXSider.onValueChanged.AddListener(SilderSFX);
    }


    private void OnDisable()
    {
        PlayerPrefs.SetFloat("music", musicSider.value);
        PlayerPrefs.SetFloat("Master", MusterSider.value);
        PlayerPrefs.SetFloat("SFX", SFXSider.value);
    }
    private void HandleSider(float value)
    {
        audioMixer.SetFloat("music", Mathf.Log10(value) * 20f);
    }
    private void SilderSFX(float value)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(value) * 20f);
    }
    private void SilderMaster(float value)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(value) * 20f);
    }
    private void Start()
    {
        changeAudio = false;
        musicSider.value = PlayerPrefs.GetFloat("music", 0f);
        MusterSider.value = PlayerPrefs.GetFloat("Master",0f);
        SFXSider.value = PlayerPrefs.GetFloat("SFX", SFXSider.value);

      
      
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
        Debug.Log("Chnage in Master");
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
    public void myReturn()
    {
        if(!changeAudio)
        {
            Debug.Log("return was call");
         //   SFXSider.value = PlayerPrefs.GetFloat("SFXVolume");
           // musicSider.value = PlayerPrefs.GetFloat("MusicVolume");
         //   MusterSider.value = PlayerPrefs.GetFloat("MusterVolume");
          //  SetFXSVolume();
         //   SetMusterVolume();
          //  SetMusicVolume();
        }
      
            GameManager.instance.ReturnFromSettings();
    }
}
