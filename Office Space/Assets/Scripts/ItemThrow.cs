using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemThrow : MonoBehaviour
{

    [SerializeField] float throwForce;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject itemSpawnPoint;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Item"))
        {
            ThrowItem();
        }
    }

    private void ThrowItem()
    {
        GameObject item = Instantiate(itemPrefab, Camera.main.transform.position, itemSpawnPoint.transform.rotation);
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
    }
}