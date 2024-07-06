using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour, IDamage
{

    //[SerializeField] enum weapon { hand, shuriken }
    //[SerializeField] weapon weaponSelection;
    [SerializeField] bool weaponSwap;

    [SerializeField] GameObject hand;
    [SerializeField] GameObject shuriken;

    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    //[SerializeField] int jumpsMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;

    Vector3 moveDir;
    Vector3 playerVel;

    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] GameObject cube;

    int jumpCount;
    int HPOrig;

    bool isShooting;

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        weaponSwap = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.green);
        WeaponHandle();
        Movement();
        Sprint();
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


    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    IEnumerator Shoot()
    {
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

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
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
