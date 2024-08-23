using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vending : MonoBehaviour, IVend
{
    [SerializeField] GameObject interactionSprite;
    [SerializeField] GameObject spotLight;
    [SerializeField] GameObject vendPos;
    [SerializeField] AudioSource Aud;
    [SerializeField] AudioSource AudSrcInteract;
    [SerializeField] AudioClip interact;

    [Header("----- Items -----")]
    [SerializeField] GameObject[] commonItems;
    [SerializeField] GameObject[] uncommonItems;
    [SerializeField] GameObject[] rareItems;
    bool playerInCollider;
    bool ambientIsPlaying;

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

        if (!ambientIsPlaying && GameManager.instance.canVend)
        {
            Aud.loop = true;
            Aud.Play();
            ambientIsPlaying = true;
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
        Aud.loop = false;
        Aud.Stop();

        int selection = Random.Range(0, 100);

        if (selection <= 15)
        {
            int index = Random.Range(0, rareItems.Length);
            Instantiate(rareItems[index], vendPos.transform.position, rareItems[index].transform.rotation);
        }
        else if (selection <= 40)
        {
            int index = Random.Range(0, uncommonItems.Length);
            Instantiate(uncommonItems[index], vendPos.transform.position, uncommonItems[index].transform.rotation);
        }
        else if (selection <= 100)
        {
            int index = Random.Range(0, commonItems.Length);
            Instantiate(commonItems[index], vendPos.transform.position, commonItems[index].transform.rotation);
        }

        AudSrcInteract.PlayOneShot(interact);
        GameManager.instance.StartVendingMachineCooldown();
        interactionSprite.SetActive(false);
        ambientIsPlaying = false;
    }
}
