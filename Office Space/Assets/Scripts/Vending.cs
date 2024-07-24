using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vending : MonoBehaviour, IVend
{
    [SerializeField] GameObject interactionSprite;
    [SerializeField] GameObject spotLight;
    [SerializeField] GameObject vendPos;
    [SerializeField] GameObject[] vendingItems;
    bool playerInCollider;

    void Update()
    {
        if (Input.GetButtonDown("Interact") && GameManager.instance.canVend && playerInCollider)
        {
            VendItem();
        }

        if (spotLight.activeSelf && !GameManager.instance.canVend)
            spotLight.SetActive(false);
        else if (!spotLight.activeSelf && GameManager.instance.canVend)
            spotLight.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.canVend)
        {
            interactionSprite.SetActive(true);
            playerInCollider = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.canVend && !interactionSprite.activeSelf)
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
        int selection = Random.Range(0, vendingItems.Length);
        Instantiate(vendingItems[selection], vendPos.transform.position, vendingItems[selection].transform.rotation);
        GameManager.instance.StartVendingMachineCooldown();
        interactionSprite.SetActive(false);
    }
}
