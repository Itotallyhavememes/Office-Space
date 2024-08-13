using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


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
    [SerializeField] float slideCooldown;
    [SerializeField] int gravity;
    //[SerializeField] int donutDropDistance;
    //[SerializeField] GameObject donutDropItem;
    [SerializeField] Camera deathCamera;
    int jumpCount;
    int HPOrig;
    float slideLockout;
    int origSpeed;
    float origHeight;
    Vector3 moveDir;
    Vector3 playerVel;

    [Header("----- Weapons -----")]
    [SerializeField] WeaponStats starterWeapon;
    [SerializeField] public List<WeaponStats> weaponList;
    [SerializeField] GameObject weaponModel;
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] float reloadTime;
    int selectedWeapon;

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
    [SerializeField] AudioClip[] audStep;
    [Range(0, 1)][SerializeField] float audStepVol;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip audSlide;
    [Range(0, 1)][SerializeField] float audSlideVol;

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
    bool isPlayingStep;
    bool isDead;
    bool canSlide;
    bool holdingDownCrouch;
    bool slideSoundPlayed;

    Coroutine speedCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        origSpeed = speed;
        origHeight = controller.height;
        slideLockout = slideLockoutTime;
        canSlide = true;

        GetWeaponStats(starterWeapon);
        DefaultPublicBools();
        //Add self to gameManager's bodyTracker
        GameManager.instance.AddToTracker(this.gameObject);
        // Call spawnPlayer
        spawnPlayer();

    }

    // Player Spawn
    public void spawnPlayer()
    {
        HP = HPOrig;
        UpdatePlayerUI();
        controller.enabled = false;
        transform.position = GameManager.instance.playerSpawn.transform.position;
        controller.enabled = true;
        if (GameManager.currentMode == GameManager.gameMode.NIGHTSHIFT)
            GameManager.instance.retryAmount = 0;
        else
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
            deathCamera.gameObject.SetActive(false);

            isDead = false;

        }
    }
    //
    public void respawnPlayer()
    {
        UpdatePlayerUI();
        controller.enabled = false;
        transform.position = GameManager.instance.playerSpawn.transform.position;
        controller.enabled = true;
        GameManager.instance.retryAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.green);
        if (!GameManager.instance.isPaused && !isDead)
        {
            if (Input.GetButtonDown("Crouch"))
                holdingDownCrouch = true;
            else if (Input.GetButtonUp("Crouch"))
                holdingDownCrouch = false;

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
        if (GameManager.instance.respawn == true)
        {
            GameManager.instance.respawn = false;
            GameManager.instance.StateUnpause();
            respawnPlayer();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.currentMode == GameManager.gameMode.NIGHTSHIFT)
        {
            HP--;
            UpdatePlayerUI();
            if (other == other.GetComponentInChildren<CapsuleCollider>() && HP < 0)
            {
                GameManager.instance.StatePause();

                GameManager.instance.ActivateMenu(GameManager.instance.menuRetryAmount);

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

        if (Input.GetButtonDown("Jump") && jumpCount < 1 && !isCrouching)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
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

        if (controller.isGrounded && moveDir.magnitude > 0.3f && !isPlayingStep)
            StartCoroutine(PlayStep());

    }

    IEnumerator PlayStep()
    {
        isPlayingStep = true;

        aud.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);

        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else if (isSprinting)
            yield return new WaitForSeconds(0.3f);

        isPlayingStep = false;
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


    public int GetSelectedWeaponIndex()
    {
        return selectedWeapon;
    }
    void Slide()
    {
        if (!slideSoundPlayed)
        {
            aud.PlayOneShot(audSlide, audSlideVol);
            slideSoundPlayed = true;
        }

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
            slideLockout -= Time.deltaTime;
        }
        else if (slideLockout <= 0)
        {
            if (!holdingDownCrouch)
                controller.height = origHeight;

            slideSoundPlayed = false;
            slideLockout = slideLockoutTime;
            isSliding = false;
            playerVel = Vector3.zero;
            speed = origSpeed;
            StartCoroutine(SlideCooldown());
        }
    }

    void Crouch()
    {
        if (canSlide && isCrouching && Input.GetButtonDown("Jump"))
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

    }

    IEnumerator SlideCooldown()
    {
        canSlide = false;
        yield return new WaitForSeconds(slideCooldown);
        canSlide = true;
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = !isSprinting;

            if (!isCrouching)
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

                yield return new WaitForSeconds(reloadTime);
                weaponModel.transform.Rotate(Vector3.right * raycastRotationReload);
                aud.PlayOneShot(audHandReloadEnd, audHandReloadEndVol);
                weaponList[selectedWeapon].currentAmmo = weaponList[selectedWeapon].startAmmo;

            }
            //else if (weaponList[selectedWeapon].type == WeaponStats.WeaponType.projectile)
            //{
            //    weaponModel.SetActive(false);
            //    yield return new WaitForSeconds(reloadTime);
            //    weaponModel.SetActive(true);

            //}
        }

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
                    else
                    {
                        Instantiate(weaponList[selectedWeapon].hitEffect, hit.point, Quaternion.identity);
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
        if (!isDead)
        {
            HP -= amount;
            StartCoroutine(flashScreenDamage());
            UpdatePlayerUI();
            aud.PlayOneShot(audDamage[Random.Range(0, audDamage.Length)], audDamageVol);

            if (HP <= 0 && GameManager.currentMode == GameManager.gameMode.NIGHTSHIFT) // 
            {
                GameManager.instance.YouLose();
            }
            else if (HP <= 0 && GameManager.currentMode == GameManager.gameMode.DONUTKING2)
            {
                //Camera.main.gameObject.SetActive(false);
                isDead = true;
                gameObject.GetComponent<CapsuleCollider>().enabled = false;
                deathCamera.gameObject.SetActive(true);
                //while (GameManager.instance.statsTracker[name] > 0)
                if (GameManager.instance.statsTracker[name].getDKStatus() == true)
                {
                    //GROUPED AS GAMEMANAGER METHOD
                    ////creates sphere that's the size of roamDist and selects a random position
                    //Vector3 randDropPos = Random.insideUnitSphere * donutDropDistance;
                    //randDropPos.y = donutDropItem.transform.position.y;
                    ////Prevents getting null reference when creating random point
                    //NavMeshHit hit;
                    ////The "1" is in refernce to layer mask "1"
                    //NavMesh.SamplePosition(randDropPos, out hit, donutDropDistance, 1);
                    //Instantiate(donutDropItem, transform.position + randDropPos, donutDropItem.transform.rotation);
                    //GameManager.instance.UpdateDonutCount(gameObject, -1);

                    GameManager.instance.dropTheDonut(this.gameObject);
                }
                GameManager.instance.DeclareSelfDead(gameObject);
                //Update Death Count in GameManager's statTracker
                GameManager.instance.statsTracker[name].updateDeaths(1);
                Debug.Log(name.ToString() + " : " + GameManager.instance.statsTracker[name].getAllStats());
                //GameManager.instance.donutCountList.Remove(gameObject.name);
            }
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
        {
            //Increment death by 1 in GameManager's statsTracker dictionary
            return true;
        }
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

    //Getters//
    public int GetStartHP()
    {
        return HPOrig;
    }

    public void GetWeaponStats(WeaponStats weapon)
    {
        int index = -1;
        foreach (var wep in weaponList)
        {
            index++;
            if (weapon.weaponModel == wep.weaponModel)
            {
                weaponList[selectedWeapon].currentAmmo = weaponList[selectedWeapon].startAmmo;
                selectedWeapon = index;
                WeaponChange();
                return;
            }
        }

        weaponList.Add(weapon);
        selectedWeapon = weaponList.Count - 1;
        weaponList[selectedWeapon].currentAmmo = weaponList[selectedWeapon].startAmmo;

        WeaponChange();

    }

    void WeaponSelect()
    {
        if (weaponList.Count <= 1)
            return;

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            selectedWeapon++;

            if (selectedWeapon > weaponList.Count - 1)
                selectedWeapon = 0;


            WeaponChange();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            selectedWeapon--;

            if (selectedWeapon < 0)
                selectedWeapon = weaponList.Count - 1;

            WeaponChange();
        }

        UpdateAmmoUI();
    }

    void WeaponChange()
    {

        shootDamage = weaponList[selectedWeapon].shootDamage;
        shootDist = weaponList[selectedWeapon].raycastDist;
        shootRate = weaponList[selectedWeapon].shootRate;
        reloadTime = weaponList[selectedWeapon].reloadTime;


        if (weaponList[selectedWeapon].type == WeaponStats.WeaponType.raycast)
        {
            shootDist = weaponList[selectedWeapon].raycastDist;
            raycastRotationReload = weaponList[selectedWeapon].raycastRotationReload;
            raycastRotationRecoil = weaponList[selectedWeapon].raycastRotationRecoil;
        }

        weaponModel.GetComponent<MeshFilter>().sharedMesh = weaponList[selectedWeapon].weaponModel.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weaponList[selectedWeapon].weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void Munch(AudioClip clip, float vol)
    {
        aud.PlayOneShot(clip, vol);
    }

}

