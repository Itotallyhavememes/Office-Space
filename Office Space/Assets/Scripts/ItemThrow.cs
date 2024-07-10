using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemThrow : MonoBehaviour
{

    [SerializeField] float throwForce;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject itemSpawnPoint;
    [SerializeField] PlayerControl player;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Item") && !player.isShooting && !player.isReloading)
        {
            ThrowItem();
        }
    }

    private void ThrowItem()
    {
        GameObject item = Instantiate(itemPrefab, itemSpawnPoint.transform.position, itemSpawnPoint.transform.rotation);
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.velocity = Camera.main.transform.forward * throwForce;
        //rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
    }
}