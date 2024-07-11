using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutCount : MonoBehaviour
{
    [SerializeField] AudioSource pickupSFX;
    TMPro.TMP_Text countText;
    int donutCount;

    // Start is called before the first frame update
    void Awake()
    {
        countText = GetComponent<TMPro.TMP_Text>();
    }

    void Start()
    {
        UpdateCount();
    }

    void OnEnable() //Enable registers OnDonutCollected method to OnCollected event
    {
        DonutPickUp.OnCollected += OnDonutCollected;
    }

    void OnDisable() //Disable unregisters OnDonutCollected method to OnCollected event
    {
        DonutPickUp.OnCollected -= OnDonutCollected;
    }


    void OnDonutCollected()
    {
        donutCount++;
        GameManager.instance.Thresh++;
        UpdateCount();
        pickupSFX.Play();
        GameManager.instance.UpdateGameGoal(1);
        GameManager.instance.playerScript.HealthPickup();
    }

    void UpdateCount()
    {
        countText.text = donutCount.ToString() + " / " + DonutPickUp.totalDonuts.ToString();
    }

}
