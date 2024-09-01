using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Vending : MonoBehaviour, IVend
{
    [SerializeField] GameObject interactionSprite;
    [SerializeField] GameObject spotLight;
    [SerializeField] GameObject vendPos;
    [SerializeField] AudioSource Aud;
    [SerializeField] AudioSource AudSrcInteract;
    [SerializeField] AudioClip interact;
    List<GameObject> inTrigger = new List<GameObject>();

    [Header("----- Items -----")]
    [SerializeField] GameObject[] commonItems;
    [SerializeField] GameObject[] uncommonItems;
    [SerializeField] GameObject[] rareItems;
    bool playerInCollider;
    bool ambientIsPlaying;

    void Update()
    {

        if (!GameManager.instance.isMultiplayer)
        {
            if (Input.GetButtonDown("Interact") && GameManager.instance.canVend && playerInCollider)
            {
                VendItem();
            }
        }
        //else
        //{
        //    foreach (var player in PlayerManager.instance.players)
        //    {
        //        if (player.GetComponent<ControllerTest>().InteractTriggered && GameManager.instance.canVend && playerInCollider)
        //        {
        //            VendItem();
        //        }
        //    }

        //}


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

        //if (GameManager.instance.canVend && inTrigger.Count > 0)
        //    interactionSprite.SetActive(true);
        //else if (!GameManager.instance.canVend && inTrigger.Count > 0)
        //    interactionSprite.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") && GameManager.instance.canVend)
        {
            foreach (var item in inTrigger)
                if (other.gameObject == item)
                    return;

            //interactionSprite.SetActive(true);
            playerInCollider = true;

            ControllerTest playerCT = other.GetComponent<ControllerTest>();

            if (playerCT != null)
            {
                playerCT.RecieveInteraction(gameObject);
                inTrigger.Add(other.gameObject);
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Player") && GameManager.instance.canVend /*&& !interactionSprite.activeSelf*/)
        {
            foreach (var person in inTrigger)
                if (other.gameObject == person)
                    return;

            //interactionSprite.SetActive(true);
            playerInCollider = true;

            ControllerTest playerCT = other.GetComponent<ControllerTest>();

            if (playerCT != null)
            {
                playerCT.RecieveInteraction(gameObject);
                inTrigger.Add(other.gameObject);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            GameObject personToRemove = other.gameObject;
            foreach (var person in inTrigger)
                if (other.gameObject == person)
                    personToRemove = person;

            ControllerTest playerCT = personToRemove.GetComponent<ControllerTest>();

            if (playerCT != null)
            {
                playerCT.RecieveInteraction(null);
                inTrigger.Remove(personToRemove);
            }

            if (inTrigger.Count <= 0)
            {
                //interactionSprite.SetActive(false);
                playerInCollider = false;
            }
        }
    }

    public void VendItem()
    {
        if (GameManager.instance.canVend && playerInCollider)
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
            //interactionSprite.SetActive(false);
            ambientIsPlaying = false;
        }
    }
}
