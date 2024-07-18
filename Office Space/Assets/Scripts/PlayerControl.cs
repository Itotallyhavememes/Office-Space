using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerControl : MonoBehaviour, IDamage, ITarget
{
    [SerializeField] GameObject hand;

    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

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

    //Hand variables
    [SerializeField] int HandAmmoCount;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] float handReloadTime;
    [SerializeField] float handRotationReload;
    [SerializeField] float handRotationRecoil;
    int handCurrentAmmo;

    //Hand Audio
    [SerializeField] AudioSource handFire;
    [SerializeField] AudioSource handReloadBegin;
    [SerializeField] AudioSource handReloadEnd;

    //Damage Audio
    [SerializeField] AudioSource DamageSound1;
    [SerializeField] AudioSource DamageSound2;
    [SerializeField] AudioSource DamageSound3;
    [SerializeField] AudioSource DamageSound4;
    [SerializeField] AudioSource DamageSound5;

    //Shuriken Variables
    [SerializeField] GameObject shurikenHUD;
    [SerializeField] GameObject shurikenSpawnPoint;
    [SerializeField] GameObject shurikenProjectile;
    [SerializeField] float shurikenRate;
    [SerializeField] int shurikenAmmo;
    [SerializeField] float shurikenReloadTime;
    int shurikenStartAmmo;

    //Item Throw
    ItemThrow item;

    public bool weaponSwap;
    public bool isShooting;
    public bool isReloading;
    bool isCrouching;
    bool isSprinting;
    bool isSliding;


    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        origSpeed = speed;
        origHeight = controller.height;
        slideLockout = slideLockoutTime * 60;
        handCurrentAmmo = HandAmmoCount;
        shurikenStartAmmo = shurikenAmmo;
        DefaultPublicBools();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.green);
        if (!GameManager.instance.isPaused)
        {
            if (!isShooting)
            {
                WeaponHandle();
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

    void WeaponHandle()
    {
        if (Input.GetButtonDown("Swap"))
        {
            weaponSwap = !weaponSwap;

            switch (weaponSwap)
            {
                case true:
                    Debug.Log("Hand");
                    shurikenHUD.SetActive(false);
                    hand.SetActive(true);
                    break;
                case false:
                    Debug.Log("Shuriken");
                    shurikenHUD.SetActive(true);
                    hand.SetActive(false);
                    break;
            }
        }
        UpdateAmmoUI();
    }

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
        if (weaponSwap && handCurrentAmmo < HandAmmoCount)
        {
            hand.transform.Rotate(Vector3.back * handRotationReload);
            handReloadBegin.Play();

            yield return new WaitForSeconds(handReloadTime);
            hand.transform.Rotate(Vector3.forward * handRotationReload);
            handReloadEnd.Play();

            handCurrentAmmo = HandAmmoCount;
        }
        else if (!weaponSwap && shurikenAmmo < shurikenStartAmmo)
        {
            shurikenHUD.SetActive(false);
            yield return new WaitForSeconds(shurikenReloadTime);
            shurikenHUD.SetActive(true);

            shurikenAmmo = shurikenStartAmmo;
        }
        isReloading = false;
        UpdateAmmoUI();
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        if (weaponSwap && handCurrentAmmo > 0)
        {
            handCurrentAmmo--;
            UpdateAmmoUI();
            handFire.Play();

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

            hand.transform.Rotate(Vector3.back * handRotationRecoil);
            yield return new WaitForSeconds(shootRate);
            hand.transform.Rotate(Vector3.forward * handRotationRecoil);

        }
        else if (!weaponSwap && shurikenAmmo > 0) //Shuriken
        {
            shurikenAmmo--;
            UpdateAmmoUI();
            shurikenHUD.SetActive(false);
            Instantiate(shurikenProjectile, shurikenSpawnPoint.transform.position, shurikenSpawnPoint.transform.rotation);
            yield return new WaitForSeconds(shurikenRate);
            shurikenHUD.SetActive(true);
        }
        else if (!isReloading && (handCurrentAmmo <= 0 || shurikenAmmo <= 0))
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
        switch (Random.Range(0, 4))
        {
            case 0:
                DamageSound1.Play();
                break;
            case 1:
                DamageSound2.Play();
                break;
            case 2:
                DamageSound3.Play();
                break;
            case 3:
                DamageSound4.Play();
                break;
            case 4:
                DamageSound5.Play();
                break;
        }

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

    //public IEnumerator SpeedPowerUp(int speedBoost) //Triggers buffs for a good cup of Joe (mediocre office brew)
    //{
    //    if (isSprinting)
    //    {
    //        speed /= sprintMod;
    //    }
    //    speed += speedBoost;
    //    yield return new WaitForSeconds(1);
    //    speed -= speedBoost;
    //}


    public void AddSpeed(int addSpeed) //Coffee Buff
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

    public void UpdatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    public void UpdateAmmoUI()
    {
        if (weaponSwap)
            GameManager.instance.playerAmmoBar.fillAmount = (float)handCurrentAmmo / HandAmmoCount;
        else if (!weaponSwap)
            GameManager.instance.playerAmmoBar.fillAmount = (float)shurikenAmmo / shurikenStartAmmo;

    }

    public GameObject declareOBJ(GameObject obj)
    {
        return gameObject;
    }

    public void HealthPickup()
    {
        int count = 1;
        while (HP < HPOrig && count <= DonutPickUp.HPincrease)
        {
            count++;
            HP++;
        }
        UpdatePlayerUI();
    }

    void DefaultPublicBools()
    {
        weaponSwap = true;
        isShooting = false;
        isReloading = false;
    }
}

