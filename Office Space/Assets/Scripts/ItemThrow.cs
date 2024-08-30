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
    [SerializeField] Camera playerCam;
    [SerializeField] ControllerTest controllerTestScript;
    [SerializeField] GameObject weaponHUD;
    [SerializeField] GameObject grenadeHUD;
    public int rubberBallCount;
    [SerializeField] int rubberBallMaxCount;

    [Header("----- Sounds -----")]
    [SerializeField] AudioClip audRubberBall;
    [Range(0, 1)][SerializeField] float audRubberBallVol;
    Animator anim;
    bool animationDone;
    void Start()
    {
        rubberBallMaxCount = rubberBallCount;
        updateGrenadeUI();
        controllerTestScript = this.GetComponent<ControllerTest>();

        if (!GameManager.instance.isMultiplayer)
            anim = player.GetComponent<Animator>();
        else
            anim = this.GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isPaused)
        {
            if (!GameManager.instance.isMultiplayer)
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
            else
            {
                if (!this.GetComponent<ControllerTest>().GetLifeState() && this.GetComponent<ControllerTest>().GrenadeTriggered && rubberBallCount > 0 && !GameManager.instance.roundEnded)
                {
                    this.GetComponent<ControllerTest>().playerRig.weight = 0;
                    anim.SetLayerWeight(8, 1f);
                    this.GetComponent<ControllerTest>().Munch(audRubberBall, audRubberBallVol);
                    anim.SetTrigger("ThrowGrenade");
                }

                if (animationDone)
                {
                    this.GetComponent<ControllerTest>().playerRig.weight = 1;
                    animationDone = false;
                }
            }
            if (this.GetComponent<ControllerTest>().GetLifeState())
            {
                weaponHUD.SetActive(false);
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
        
        if (!GameManager.instance.isMultiplayer)
            rb.velocity = Camera.main.transform.forward * throwForce;
        else
            rb.velocity = playerCam.transform.forward * throwForce;

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

        if(!GameManager.instance.isMultiplayer)
            rb.velocity = Camera.main.transform.forward * throwForce;
        else
            rb.velocity = playerCam.transform.forward * throwForce;

        grenadeHUD.SetActive(false);
        rubberBallCount--;
        updateGrenadeUI();
        yield return new WaitForSeconds(throwDelay);
        WeaponToggleOn();        
    }

    void WeaponToggleOff()
    {
        if (!GameManager.instance.isMultiplayer)
        {
            player.isShooting = true;
            player.isReloading = true;
        }
        else
        {
            this.GetComponent<ControllerTest>().isShooting = true;
            this.GetComponent<ControllerTest>().isReloading = true;
        }
        weaponHUD.SetActive(false);
        grenadeHUD.SetActive(true);

    }

    void WeaponToggleOn()
    {
        weaponHUD.SetActive(true);
        if (!GameManager.instance.isMultiplayer)
        {
            player.isShooting = false;
            player.isReloading = false;
        }
        else
        {
            this.GetComponent<ControllerTest>().isShooting = false;
            this.GetComponent<ControllerTest>().isReloading = false;
        }
    }
    public void updateGrenadeUI() //Old player input method
    {
        if (!GameManager.instance.isMultiplayer)
            GameManager.instance.grenadeStack.text = (rubberBallCount).ToString();
        else
            this.GetComponent<ControllerTest>().grenadeStack.text = (rubberBallCount).ToString();
    }

    public int GetMaxBallCount()
    {
        return rubberBallMaxCount;
    }
}