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
    [SerializeField] GameObject weaponHUD;
    [SerializeField] GameObject grenadeHUD;

    public int rubberBallCount;
    int rubberBallMaxCount;

    void Start()
    {
        rubberBallMaxCount = rubberBallCount;
        updateGrenadeUI();

    }
    // Update is called once per frame
    void Update()
    {
        if (!player.isShooting && !player.isReloading)
        {
            if (Input.GetButtonDown("Item") && rubberBallCount > 0)
            {
                StartCoroutine(ThrowItem());
            }
        }
    }

    public int GetRubberBallMax()
    {
        return rubberBallMaxCount;
    }

    IEnumerator ThrowItem()
    {
        WeaponToggleOff();
        yield return new WaitForSeconds(throwDelay);
        GameObject item = Instantiate(itemPrefab, itemSpawnPoint.transform.position, itemSpawnPoint.transform.rotation);
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.velocity = Camera.main.transform.forward * throwForce;
        grenadeHUD.SetActive(false);
        rubberBallCount--;
        updateGrenadeUI();
        yield return new WaitForSeconds(throwDelay);
        WeaponToggleOn();
    }

    void WeaponToggleOff()
    {
        player.isShooting = true;
        player.isReloading = true;
        weaponHUD.SetActive(false);
        grenadeHUD.SetActive(true);

    }

    void WeaponToggleOn()
    {
        weaponHUD.SetActive(true);
        player.isShooting = false;
        player.isReloading = false;
    }
    public void updateGrenadeUI()
    {
        GameManager.instance.grenadeStack.text = (rubberBallCount).ToString();
    }

    public int GetMaxBallCount()
    {
        return rubberBallMaxCount;
    }
}