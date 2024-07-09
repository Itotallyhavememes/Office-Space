using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour, IDamage
{

    //[SerializeField] enum weapon { hand, shuriken }
    //[SerializeField] weapon weaponSelection;

    [SerializeField] GameObject hand;

    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int crouchMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;

    Vector3 moveDir;
    Vector3 playerVel;

    [SerializeField] int ammoCount;
    [SerializeField] int handCurrentAmmo;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] AudioSource handFire;
    [SerializeField] AudioSource handReloadBegin;
    [SerializeField] AudioSource handReloadEnd;

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
    int shurikenStartAmmo;



    int jumpCount;
    int HPOrig;

    [SerializeField] float crouchLevel;
    [SerializeField] int slideSpeed;
    [SerializeField] float slideLockoutTime;
    float slideLockout;
    int origSpeed;
    float origHeight;

    bool weaponSwap;
    bool isShooting;
    bool isCrouching;
    bool isSprinting;
    bool isSliding;

    [SerializeField] float handReloadTime;
    [SerializeField] float handRotationReload;
    [SerializeField] float handRotationRecoil;

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        origSpeed = speed;
        weaponSwap = true;
        origHeight = controller.height;
        slideLockout = slideLockoutTime * 60;
        handCurrentAmmo = ammoCount;
        shurikenStartAmmo = shurikenAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.green);
        if (!GameManager.instance.isPaused)
        {
            WeaponHandle();
            if (!isSliding)
            {
                Movement();
                Crouch();
                Sprint();
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

        if (Input.GetButton("Fire1") && !isShooting)
        {
            StartCoroutine(Shoot());
        }

        if (Input.GetButtonDown("Reload") && handCurrentAmmo < ammoCount)
        {
            StartCoroutine(Reload());
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
    }

    void Slide()
    {
        isCrouching = true;
        isSprinting = false;
        controller.height = crouchLevel;
        playerVel = transform.forward *= slideSpeed;
        controller.Move(playerVel * Time.deltaTime);
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
        }
    }

    void Crouch()
    {
        if (isSprinting && Input.GetButtonDown("Crouch") && controller.isGrounded)
        {
            isSliding = true;
        }
        else if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = !isCrouching;

            switch (isCrouching)
            {
                case true:
                    controller.height = crouchLevel;
                    speed -= crouchMod;
                    break;
                case false:
                    controller.height = origHeight;
                    speed += crouchMod;
                    break;
            }
        }

        if (isCrouching)
            if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Sprint"))
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
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed = origSpeed;
            isSprinting = false;
        }
    }

    IEnumerator Reload()
    {
        if (weaponSwap)
        {
            hand.transform.Rotate(Vector3.back * handRotationReload);
            handReloadBegin.Play();

            handCurrentAmmo = ammoCount;

            yield return new WaitForSeconds(handReloadTime);
            hand.transform.Rotate(Vector3.forward * handRotationReload);
            handReloadEnd.Play();
        }
        else if (!weaponSwap)
        {
            shurikenAmmo = shurikenStartAmmo;
            shurikenHUD.SetActive(false);
            yield return new WaitForSeconds(shurikenRate);
            shurikenHUD.SetActive(true);
        }

    }

    IEnumerator Shoot()
    {
        if (handCurrentAmmo <= 0 || shurikenAmmo <= 0)
        {
            StartCoroutine(Reload());
            yield return null;
        }

        isShooting = true;

        if (weaponSwap && handCurrentAmmo > 0)
        {
            handCurrentAmmo--;

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
            shurikenHUD.SetActive(false);
            Instantiate(shurikenProjectile, shurikenSpawnPoint.transform.position, shurikenSpawnPoint.transform.rotation);
            yield return new WaitForSeconds(shurikenRate);
            shurikenHUD.SetActive(true);
        }
        

        isShooting = false;

    }

    public void takeDamage(int amount)
    {
        HP -= amount;

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
}
