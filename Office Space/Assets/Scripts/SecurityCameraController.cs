using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraController : MonoBehaviour
{
    [SerializeField] float maxTime;

    float totalTime;
    bool playerSpotted = false;


    // Update is called once per frame
    void Update()
    {
        if (playerSpotted)
        {
            totalTime += Time.deltaTime;
            if(totalTime > maxTime)
            {
                alertEnemies();
            }
        }
        if (!playerSpotted)
        {
            totalTime--;//Reduces time in camera but not instantly so if you go out and right back in it compounds
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSpotted = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSpotted = false;
        }
    }



    public void alertEnemies()
    {

    }
}
