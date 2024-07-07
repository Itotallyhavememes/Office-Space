using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour, IDamage
{

    //[SerializeField] enum weapon { hand, shuriken }
    //[SerializeField] weapon weaponSelection;

    [SerializeField] GameObject hand;
    [SerializeField] GameObject shuriken;

    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int crouchMod;
    //[SerializeField] int jumpsMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;

    Vector3 moveDir;
    Vector3 playerVel;

    [SerializeField] int ammoCount;
    [SerializeField] int currentAmmo;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] GameObject cube;


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
        currentAmmo = ammoCount;
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
        moveDir = Input.GetAxis("Vertical") * transform.forward +
                   Input.GetAxis("Horizontal") * transform.right;
        controller.Move(moveDir * speed * Time.deltaTime);

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        if (Input.GetButtonDown("Jump") && jumpCount < 1 /*jumpsMax*/)
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

        if (Input.GetButtonDown("Reload") && currentAmmo < ammoCount)
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
                    //shuriken.SetActive(false);
                    hand.SetActive(true);
                    break;
                case false:
                    Debug.Log("Shuriken");
                    //shuriken.SetActive(true);
                    hand.SetActive(false);
                    break;
            }
        }
    }

    void Slide()
    {
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
            hand.transform.Rotate(Vector3.back * handRotationReload);

        currentAmmo = ammoCount;

        yield return new WaitForSeconds(handReloadTime);

        if (weaponSwap)
            hand.transform.Rotate(Vector3.forward * handRotationReload);
    }

    IEnumerator Shoot()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            isShooting = true;

            if (weaponSwap)
            {

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
            }
            else if (!weaponSwap)
            {
                //shuriken code here
            }

            if (weaponSwap)
                hand.transform.Rotate(Vector3.back * handRotationRecoil);

            yield return new WaitForSeconds(shootRate);

            if (weaponSwap)
                hand.transform.Rotate(Vector3.forward * handRotationRecoil);

            isShooting = false;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            GameManager.instance.YouLose();
        }
    }
}
