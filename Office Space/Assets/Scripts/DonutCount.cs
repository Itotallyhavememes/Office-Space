using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutCount : MonoBehaviour
{
    TMPro.TMP_Text countText;
    int donutCount;

    // Start is called before the first frame update
    void Awake()
    {
        countText = GetComponent<TMPro.TMP_Text>();
    }

    private void Start()
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
        UpdateCount();
    }

    void UpdateCount()
    {
        countText.text = donutCount.ToString() + " / " + DonutPickUp.totalDonuts.ToString();
    }

}
