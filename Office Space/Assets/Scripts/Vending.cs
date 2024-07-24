using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vending : MonoBehaviour, IVend
{
    [SerializeField] GameObject interactionSprite;
    [SerializeField] Transform vendPos;
    [SerializeField] GameObject[] vendingItems;
    bool playerInCollider;

    void Update()
    {
        if (Input.GetButtonDown("Interact") && GameManager.instance.canVend && playerInCollider)
        {
            VendItem();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.canVend)
        {
            interactionSprite.SetActive(true);
            playerInCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionSprite.SetActive(false);
            playerInCollider = false;
        }
    }

    public void VendItem()
    { 

    }
}
