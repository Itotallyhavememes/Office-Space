using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ItemThrow : MonoBehaviour
{

    [SerializeField] float throwForce;
    [SerializeField] float throwDelay;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject itemSpawnPoint;
    [SerializeField] PlayerControl player;
    [SerializeField] GameObject weaponHUD;
    [SerializeField] GameObject grenadeHUD;
    [Header("----- Sounds -----")]
    [SerializeField] AudioClip audRubberBall;
    [SerializeField] AudioSource aud;
    [Range(0, 1)][SerializeField] float audRubberBallVol;
    public int rubberBallCount;
    int rubberBallMaxCount;
    Animator anim;
    bool animationDone;
    void Start()
    {
        rubberBallMaxCount = rubberBallCount;
        updateGrenadeUI();
        anim = player.GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        if (animationDone)
        {
            player.playerRig.weight = 1;
            animationDone = false;
        }

        if (!player.isShooting && !player.isReloading)
        {
            if (Input.GetButtonDown("Item") && rubberBallCount > 0)
            {
                //StartCoroutine(ThrowItem());
                player.playerRig.weight = 0;
                anim.SetLayerWeight(8, 1f);
                GameManager.instance.playerScript.Munch(audRubberBall, audRubberBallVol);
                anim.SetTrigger("ThrowGrenade");
            }
        }
    }

    public void AnimationDone()
    {
        animationDone = true;
        anim.SetLayerWeight(8, 0);
        WeaponToggleOn();
    }
    public void ThrowGrenade()
    {
         
        grenadeHUD.SetActive(false);
        GameObject item = Instantiate(itemPrefab, itemSpawnPoint.transform.position, itemSpawnPoint.transform.rotation);
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.velocity = Camera.main.transform.forward * throwForce;
        rubberBallCount--;
        updateGrenadeUI();
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