using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemThrow : MonoBehaviour
{

    [SerializeField] float throwForce;
    [SerializeField] float throwDelay;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject itemSpawnPoint;
    [SerializeField] PlayerControl player;
    [SerializeField] GameObject handHUD;
    [SerializeField] GameObject shurikenHUD;
    [SerializeField] GameObject grenadeHUD;

    public int rubberBallCount;
    int rubberBallStartCount;

    private void Start()
    {
        rubberBallStartCount = rubberBallCount;
    }
    // Update is called once per frame
    void Update()
    {
        if (!player.isShooting && !player.isReloading)
        {
            if (Input.GetButtonDown("Item") && rubberBallCount > 0)
            {
                StartCoroutine(ThrowItem());
                rubberBallCount--;
            }
        }
    }

    IEnumerator ThrowItem()
    {
        WeaponToggleOff();
        yield return new WaitForSeconds(throwDelay);
        GameObject item = Instantiate(itemPrefab, itemSpawnPoint.transform.position, itemSpawnPoint.transform.rotation);
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.velocity = Camera.main.transform.forward * throwForce;
        grenadeHUD.SetActive(false);
        yield return new WaitForSeconds(throwDelay);
        WeaponToggleOn();

    }

    void WeaponToggleOff()
    {
        player.isShooting = true;
        player.isReloading = true;
        if (player.weaponSwap)
        {
            handHUD.SetActive(false);
        }
        else
        {
            shurikenHUD.SetActive(false);
        }
        grenadeHUD.SetActive(true);
        
    }

    void WeaponToggleOn()
    {
        if (player.weaponSwap)
        {
            handHUD.SetActive(true);
        }
        else
        {
            shurikenHUD.SetActive(true);
        }
        player.isShooting = false;
        player.isReloading = false;
    }
}