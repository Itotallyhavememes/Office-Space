using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManger : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;


    private void Start()
    {
        musicSource.Play();
    }


}
