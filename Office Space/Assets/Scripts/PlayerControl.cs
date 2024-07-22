using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerControl : MonoBehaviour, IDamage, ITarget
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;
    [SerializeField] LayerMask ignoreMask;

    [Header("----- Variables -----")]

    //Player variables
    public int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int crouchMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] float crouchLevel;
    [SerializeField] int slideSpeed;
    [SerializeField] float slideLockoutTime;
    [SerializeField] int gravity;
    int jumpCount;
    int HPOrig;
    float slideLockout;
    int origSpeed;
    float origHeight;
    Vector3 moveDir;
    Vector3 playerVel;

    [Header("----- Weapons -----")]
    [SerializeField] WeaponStats starterWeapon;
    int selectedWeapon;
    [SerializeField] List<WeaponStats> weaponList;
    [SerializeField] GameObject weaponModel;
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] float reloadtime;

    [SerializeField] float shootDist;
    [SerializeField] float raycastRotationReload;
    [SerializeField] float raycastRotationRecoil;

    [Header("----- Shuriken -----")]
    //Shuriken Variables
    [SerializeField] GameObject shurikenSpawnPoint;
    [SerializeField] GameObject shurikenProjectile;
    //[SerializeField] GameObject shurikenHUD;
    //[SerializeField] float shurikenRate;
    //[SerializeField] float shurikenReloadTime;
    //public int shurikenAmmo;
    //int shurikenStartAmmo;

    [Header("----- Hand -----")]

    //Hand variables
    //[SerializeField] GameObject hand;
    //[SerializeField] float handReloadTime;

    [Header("----- Sounds -----")]

    //Hand Audio
    [SerializeField] AudioClip audHandFire;
    [Range(0, 1)][SerializeField] float audHandFireVol;
    [SerializeField] AudioClip audHandReloadBegin;
    [Range(0, 1)][SerializeField] float audHandReloadBeginVol;
    [SerializeField] AudioClip audHandReloadEnd;
    [Range(0, 1)][SerializeField] float audHandReloadEndVol;

    //Damage Audio
    [SerializeField] AudioClip[] audDamage;
    [Range(0, 1)][SerializeField] float audDamageVol;

    //Item Throw
    ItemThrow item;

    public bool weaponSwap;
    public bool isShooting;
    public bool isReloading;
    bool isCrouching;
    bool isSprinting;
    bool isSliding;

    Coroutine speedCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        origSpeed = speed;
        origHeight = controller.height;
        slideLockout = slideLockoutTime * 60;
        GetWeaponStats(starterWeapon);
        DefaultPublicBools();
        //Add self to gameManager's bodyTracker
        GameManager.instance.AddToTracker(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.green);
        if (!GameManager.instance.isPaused)
        {
            if (!isShooting)
            {
                WeaponSelect();
            }

            if (!isSliding)
            {
                Movement();
                Sprint();
                Crouch();
            }
            else if (isSliding)
            {
                Slide();
            }
        }
    }

    void Movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = Input.GetAxis("Vertical") * transform.forward +
                   Input.GetAxis("Horizontal") * transform.right;
        controller.Move(moveDir * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && jumpCount < 1)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (!isShooting && !isReloading)
        {
            if (Input.GetButton("Fire1"))
            {
                StartCoroutine(Shoot());
            }

            if (Input.GetButtonDown("Reload"))
            {
                StartCoroutine(Reload());
            }

        }

    }

    //void WeaponHandle()
    //{
    //    if (Input.GetButtonDown("Swap"))
    //    {
    //        weaponSwap = !weaponSwap;

    //        switch (weaponSwap)
    //        {
    //            case true:
    //                Debug.Log("Hand");
    //                shurikenHUD.SetActive(false);
    //                hand.SetActive(true);
    //                break;
    //            case false:
    //                Debug.Log("Shuriken");
    //                shurikenHUD.SetActive(true);
    //                hand.SetActive(false);
    //                break;
    //        }
    //    }
    //    UpdateAmmoUI();
    //}

    void Slide()
    {
        isCrouching = true;
        isSprinting = false;

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        controller.height = crouchLevel;

        playerVel.x = transform.forward.x * slideSpeed;
        playerVel.z = transform.forward.z * slideSpeed;

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (slideLockout > 0)
        {
            slideLockout--;
        }
        else if (slideLockout <= 0)
        {
            slideLockout = slideLockoutTime * 60;
            controller.height = origHeight;
            isCrouching = false;
            isSliding = false;
            playerVel = Vector3.zero;
            speed = origSpeed;
            Sprint();
        }
    }

    void Crouch()
    {
        if (isSprinting && Input.GetButtonDown("Crouch"))
        {
            isSliding = true;
        }
        else if (Input.GetButtonDown("Crouch"))
        {
            controller.height = crouchLevel;
            speed -= crouchMod;
            isCrouching = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            controller.height = origHeight;
            speed += crouchMod;
            isCrouching = false;
        }

        if (isCrouching && (Input.GetButtonDown("Jump") || Input.GetButtonDown("Sprint")))
        {
            isCrouching = false;
            controller.height = origHeight;
            speed += crouchMod;
        }
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = !isSprinting;

            switch (isSprinting)
            {
                case true:
                    speed *= sprintMod;
                    break;
                case false:
                    speed /= sprintMod;
                    break;
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (weaponList[selectedWeapon].currentAmmo < weaponList[selectedWeapon].startAmmo)
        {
            if (weaponList[selectedWeapon].type == WeaponStats.WeaponType.raycast)
            {
                weaponModel.transform.Rotate(Vector3.left * raycastRotationReload);
                aud.PlayOneShot(audHandReloadBegin, audHandReloadBeginVol);

                yield return new WaitForSeconds(reloadtime);
                weaponModel.transform.Rotate(Vector3.right * raycastRotationReload);
                aud.PlayOneShot(audHandReloadEnd, audHandReloadEndVol);

            }
            else if (weaponList[selectedWeapon].type == WeaponStats.WeaponType.projectile)
            {
                weaponModel.SetActive(false);
                yield return new WaitForSeconds(reloadtime);
                weaponModel.SetActive(true);

            }
        }
        weaponList[selectedWeapon].currentAmmo = weaponList[selectedWeapon].startAmmo;
        isReloading = false;
        UpdateAmmoUI();
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        if (weaponList[selectedWeapon].currentAmmo > 0)
        {
            weaponList[selectedWeapon].currentAmmo--;
            UpdateAmmoUI();

            if (weaponList[selectedWeapon].type == WeaponStats.WeaponType.raycast)
            {
                aud.PlayOneShot(audHandFire, audHandFireVol);

                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
                {
                    Debug.Log(hit.collider.name);

                    IDamage dmg = hit.collider.GetComponent<IDamage>();

                    if (hit.transform != transform && dmg != null)
                    {
                        dmg.takeDamage(shootDamage);
                    }
                }

                Debug.Log("Rotating arm");
                weaponModel.transform.Rotate(Vector3.left * raycastRotationRecoil);
                yield return new WaitForSeconds(shootRate);
                weaponModel.transform.Rotate(Vector3.right * raycastRotationRecoil);

            }
            else if (weaponList[selectedWeapon].type == WeaponStats.WeaponType.projectile) //Shuriken
            {
                weaponModel.SetActive(false);
                Instantiate(shurikenProjectile, shurikenSpawnPoint.transform.position, shurikenSpawnPoint.transform.rotation);
                yield return new WaitForSeconds(shootRate);
                weaponModel.SetActive(true);
            }
        }
        else if (!isReloading && (weaponList[selectedWeapon].currentAmmo <= 0))
        {
            StartCoroutine(Reload());
        }

        isShooting = false;

    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashScreenDamage());
        UpdatePlayerUI();
        aud.PlayOneShot(audDamage[Random.Range(0, audDamage.Length)], audDamageVol);

        if (HP <= 0)
        {
            GameManager.instance.YouLose();
        }
    }

    IEnumerator flashScreenDamage()
    {
        GameManager.instance.damageFlash.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.damageFlash.SetActive(false);
    }


    IEnumerator SpeedPowerUp(SpeedBuff stats) //Triggers buffs for a good cup of Joe (mediocre office brew)
    {
        AddSpeed(stats.speedModifier);
        yield return new WaitForSeconds(stats.speedBoostTime);
        AddSpeed(-stats.speedModifier);
    }

    private void AddSpeed(int addSpeed) //Controls the speed increase and maintains the boost constant regardless of state of player movement
    {
        if (isSprinting)
        {
            if (addSpeed > 0)
                speed = (origSpeed + addSpeed) * sprintMod;
            else
                speed = origSpeed * sprintMod;
        }
        else
        {
            if (addSpeed > 0)
                speed = origSpeed + addSpeed;
            else
                speed = origSpeed;
        }
    }

    public void ActivateSpeedBoost(SpeedBuff stats) //Handles the activation of the speed power up coroutine 
    {
        if (speedCoroutine != null) //resets the timer if player picks up another power up
        {
            StopCoroutine(speedCoroutine);
        }
        speedCoroutine = StartCoroutine(SpeedPowerUp(stats));
    }


    public void UpdatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    public void UpdateAmmoUI()
    {
            GameManager.instance.playerAmmoBar.fillAmount = (float)weaponList[selectedWeapon].currentAmmo / weaponList[selectedWeapon].startAmmo;

    }
    //ITarget Specific Methods
    public GameObject declareOBJ(GameObject obj)
    {
        return gameObject;
    }

    public bool declareDeath()
    {
        if (HP <= 0)
            return true;
        else
            return false;
    }
    //END ITarget Methods

    public void HealthPickup() //Proto 1 HealthPickup
    {
        int count = 1;
        while (HP < HPOrig && count <= DonutPickUp.HPincrease)
        {
            count++;
            HP++;
        }
        UpdatePlayerUI();
    }

    public void HealthPickup(int amount) //PowerUp Health Pickup
    {
        HP += amount;
        if (HP > HPOrig)
        {
            HP = HPOrig;
        }
        UpdatePlayerUI();
    }

    void DefaultPublicBools()
    {
        weaponSwap = true;
        isShooting = false;
        isReloading = false;
    }

    //public int GetStartShurikenAmmo()
    //{
    //    return shurikenStartAmmo;
    //}

    public int GetStartHP()
    {
        return HPOrig;
    }

    public void GetWeaponStats(WeaponStats weapon)
    {
        weaponList.Add(weapon);
        selectedWeapon = weaponList.Count - 1;

        WeaponChange();

    }

    void WeaponSelect()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedWeapon < weaponList.Count - 1)
        {
            selectedWeapon++;
            WeaponChange();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedWeapon > 0)
        {
            selectedWeapon--;
            WeaponChange();
        }
    }

    void WeaponChange()
    {

        shootDamage = weaponList[selectedWeapon].shootDamage;
        shootDist = weaponList[selectedWeapon].raycastDist;
        shootRate = weaponList[selectedWeapon].shootRate;

        weaponModel.GetComponent<MeshFilter>().sharedMesh = weaponList[selectedWeapon].weaponModel.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weaponList[selectedWeapon].weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
    }


}

