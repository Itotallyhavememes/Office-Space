using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class ControllerTest : MonoBehaviour //ITarget //, IDamage 
{
    [Header("Input Action Asset")]
    [SerializeField] InputActionAsset inputAsset;


    [Header("----- Components -----")]
    [SerializeField] CharacterController characterController;
    [SerializeField] AudioSource aud;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Animator anim;
    [SerializeField] float animTransitSpeed;
    float agentSpeedVert;
    float agentSpeedHori;
    bool throwAnimDone;
    public Rig playerRig;

    [Header("Player Mesh and Player Effects")]
    [SerializeField] SkinnedMeshRenderer playerMeshRenderer;
    [SerializeField] Color dmgColor;
    [SerializeField] GameObject DKLight;
    Color origColor;

    [Header("Movement Parameters")]
    [SerializeField] float speed = 6.0f;
    [SerializeField] float sprintMod = 2.0f;
    private Vector3 playerVel;
    float origSpeed;
    bool isSprinting;

    [Header("Crouch Parameters")]
    [SerializeField] float crouchLevel;
    [SerializeField] float crouchMod;
    bool isCrouching;
    float origHeight;
    float crouchSpeed;

    [Header("Jump Parameters")]
    [SerializeField] float jumpForce = 8.0f;
    [SerializeField] float gravity = 22.0f;
    int jumpCount;

    [Header("Sliding Parameters")]
    [SerializeField] int slideSpeed;
    [SerializeField] float slideLockoutTime;
    [SerializeField] float slideCooldown;
    bool slideSoundPlayed;
    bool slideAnimPlayed;
    float slideLockout;
    bool isSliding;
    bool canSlide;

    [Header("Camera Settings")]
    [SerializeField] Camera playerCamera;
    [SerializeField] GameObject cameraLockOn;
    [SerializeField] bool invertYAxis = false;

    [Header("HP Parameters")]
    public int HP;
    private int HPOrig;

    [Header("Player UI")]
    [SerializeField] MultiplayerEventSystem eventSystem;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject firstSelectedButtonInPause;
    public Image playerHPBar;
    public Image playerAmmoBar;

    [Header("Death Cam")]
    [SerializeField] Camera deathCamera;
    bool isDead;

    [Header("----- Weapons -----")]
    [SerializeField] GameObject playerAim;
    [SerializeField] float aimBallDist;
    [SerializeField] GameObject weaponHUD;
    [SerializeField] WeaponStats starterWeapon;
    [SerializeField] public List<WeaponStats> weaponList;
    [SerializeField] GameObject weaponModel;
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] float reloadTime;
    [SerializeField] float shootDist;
    [SerializeField] float raycastRotationRecoil;
    [SerializeField] float raycastRotationReload;
    [SerializeField] int currentAmmo;

    [Header("----- Shuriken -----")]
    [SerializeField] GameObject shurikenSpawnPoint;
    [SerializeField] GameObject shurikenProjectile;
    public bool isReloading;
    public bool isShooting;
    int selectedWeapon;

    [Header("----- Sounds -----")]
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
    [SerializeField] AudioClip[] audDamage;
    [Range(0, 1)][SerializeField] float audDamageVol;
    [SerializeField] AudioClip audRubberBall;
    [Range(0, 1)][SerializeField] float audRubberBallVol;

    [Header("---- Grenade ----")]
    [SerializeField] GameObject grenadeHUD;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject itemSpawnPoint;
    [SerializeField] float throwForce;
    [SerializeField] float throwDelay;
    public int rubberBallCount;
    int rubberBallMaxCount;
    ItemThrow item;


    [Header("Look Sensitivity")]
    [SerializeField] float mouseSensitivity = 2.0f;
    [SerializeField] float upDownRange = 90.0f;
    [SerializeField] Camera playerCam;
    private float verticalRotation;

    [Header("Action Map Name References")]
    [SerializeField] string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] string movement = "Movement";
    [SerializeField] string look = "Look";
    [SerializeField] string jump = "Jump";
    [SerializeField] string sprint = "Sprint";
    [SerializeField] string shoot = "Shoot";
    [SerializeField] string reload = "Reload";
    [SerializeField] string grenade = "Grenade";
    [SerializeField] string join = "Join";
    [SerializeField] string pause = "Pause";
    [SerializeField] string resume = "Cancel";
    [SerializeField] string crouch = "Crouch";
    [SerializeField] string aim = "Aim";
    [SerializeField] string adsLeft = "ADS Left";
    [SerializeField] string adsRight = "ADS Right";
    [SerializeField] string swapWeaponsCont = "Swap Weapon";
    [SerializeField] string swapWeaponsScroll = "Swap Weapon Scroll";
    [SerializeField] string interact = "Interact";
    [SerializeField] string scoreboard = "Scroeboard";

    [Header("Deadzone Values")]
    [SerializeField] float leftStickDeadzoneValue = 0.2f;

    InputActionMap player;
    InputAction movementAction;
    InputAction lookAction;
    InputAction jumpAction;
    InputAction sprintAction;
    InputAction shootAction;
    InputAction reloadAction;
    InputAction grenadeAction;
    InputAction cycleWeaponAction;
    InputAction joinAction;
    InputAction pauseAction;
    InputAction resumeAction;
    InputAction crouchAction;
    InputAction aimAction;
    InputAction adsLeftAction;
    InputAction adsRightAction;
    InputAction swapWeaponsContAction;
    InputAction swapWeaponsScrollAction;
    InputAction interactAction;
    InputAction scoreboardAction;

    //auto-implemented property with a get and set accessor. Can be read from anywhere (public), but can only be set from within the class (private)
    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool SprintTrigger { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool CrouchTrigger { get; private set; }
    public float SprintValue { get; private set; }
    public bool ShootTriggered { get; private set; }
    public bool GrenadeTriggered { get; private set; }
    public bool CycleWeaponTriggered { get; private set; }
    public bool SplitCameraTriggered { get; private set; }
    public bool AimTriggered { get; private set; }
    public bool AdsLeftTriggered { get; private set; }
    public bool adsRightTriggered { get; private set; }
    public bool SwapWeaponsTriggered { get; private set; }
    public bool InteractTriggered { get; private set; }
    public bool ScoreboardTriggered { get; private set; }


    private void Awake()
    {

        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap(actionMapName);

        //allows us to reference the action and find them  
        movementAction = player.FindAction(movement);
        lookAction = player.FindAction(look);
        jumpAction = player.FindAction(jump);
        crouchAction = player.FindAction(crouch);
        sprintAction = player.FindAction(sprint);
        shootAction = player.FindAction(shoot);
        reloadAction = player.FindAction(reload);
        grenadeAction = player.FindAction(grenade);
        joinAction = player.FindAction(join);
        pauseAction = player.FindAction(pause);
        aimAction = player.FindAction(aim);
        adsLeftAction = player.FindAction(adsLeft);
        adsRightAction = player.FindAction(adsRight);
        swapWeaponsContAction = player.FindAction(swapWeaponsCont);
        swapWeaponsScrollAction = player.FindAction(swapWeaponsScroll);
        interactAction = player.FindAction(interact);
        scoreboardAction = player.FindAction(scoreboard);
        //splitCameraAction = player.FindAction(splitCamera); //testing

        RegisterInputActions();

        InputSystem.settings.defaultDeadzoneMin = leftStickDeadzoneValue;

        eventSystem = FindObjectOfType<MultiplayerEventSystem>();
    }
    private void Start()
    {
        HPOrig = HP;
        origSpeed = speed;
        origHeight = characterController.height;
        slideLockout = slideLockoutTime;
        crouchSpeed = speed - crouchMod;
        DKLight.SetActive(false);
        origColor = playerMeshRenderer.material.color;
        GetWeaponStats(starterWeapon);
        //Add self to gameManager's bodyTracker
        GameManager.instance.AddToTracker(this.gameObject);
        // Don't need to call spawn player cause player manager does it for me
        // rubberBallMaxCount = rubberBallCount;
        updateGrenadeUI();
    }

    private void Update()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * shootDist, Color.green);

        if (PlayerManager.instance.matchStarted && !GameManager.instance.isPaused)
        {
            Movement();
            Rotation();
        }

        //----------------This should NOT be here--------------------
        //if (!isShooting && !isReloading)
        //{
        //    if (GrenadeTriggered && rubberBallCount > 0)
        //    {
        //        StartCoroutine(ThrowItem());
        //    }
        //}
        //----------------This should NOT be here--------------------


        


        if (!isCrouching)
        {
            anim.SetLayerWeight(1, 1);
            anim.SetLayerWeight(2, 1);
            anim.SetLayerWeight(3, 0);
            anim.SetLayerWeight(4, 0);
        }
        else if (isCrouching)
        {
            anim.SetLayerWeight(1, 0);
            anim.SetLayerWeight(2, 0);
            anim.SetLayerWeight(3, 1);
            anim.SetLayerWeight(4, 1);
        }
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agentSpeedVert, Time.deltaTime * animTransitSpeed));
        anim.SetFloat("SpeedHori", Mathf.Lerp(anim.GetFloat("SpeedHori"), agentSpeedHori, Time.deltaTime * animTransitSpeed));

        playerAim.transform.position = playerCamera.transform.position + (playerCamera.transform.forward * aimBallDist);
        playerAim.transform.rotation = playerCamera.transform.rotation;

    }

    //Registers each action and assigns them a value (Event Table) Only for actions not mapped to a method
    void RegisterInputActions()
    {
        movementAction.performed += context => MovementInput = context.ReadValue<Vector2>();
        movementAction.canceled += context => MovementInput = Vector2.zero;

        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        jumpAction.performed += context => JumpTriggered = true;
        jumpAction.canceled += context => JumpTriggered = false;

        //crouchAction.performed += context => CrouchTrigger = true;
        //crouchAction.canceled += context => CrouchTrigger = false;

        //sprintAction.performed += context => SprintValue = context.ReadValue<float>();
        //sprintAction.canceled += context => SprintValue = 0f;

        sprintAction.performed += context => SprintTrigger = true;
        sprintAction.canceled += context => SprintTrigger = false;

        //shootAction.performed += context => ShootTriggered = true;
        //shootAction.canceled += context => ShootTriggered = false;

        //reloadAction.performed += context => ReloadTriggered = true;
        //reloadAction.canceled += context => ReloadTriggered = false;

        grenadeAction.performed += context => GrenadeTriggered = true;
        grenadeAction.canceled += context => GrenadeTriggered = false;

        aimAction.performed += context => AimTriggered = true;
        aimAction.canceled += context => AimTriggered = false;

        adsLeftAction.performed += context => AdsLeftTriggered = true;
        adsLeftAction.canceled += context => AdsLeftTriggered = false;

        adsRightAction.performed += context => adsRightTriggered = true;
        adsRightAction.canceled += context => adsRightTriggered = false;

        interactAction.performed += context => InteractTriggered = true;
        interactAction.canceled += context => InteractTriggered = false;

        scoreboardAction.performed += context => ScoreboardTriggered = true;
        scoreboardAction.canceled += context => ScoreboardTriggered = false;

    }

    //On Enable and Disable is required as this input manager uses an event handler
    private void OnEnable()
    {
        //movementAction.performed += OnMovement;
        //movementAction.canceled += OnMovementStop;
        movementAction.Enable();

        lookAction.Enable();
        //jumpAction.performed += OnJumpStart;
        jumpAction.Enable();
        crouchAction.performed += Crouch;
        crouchAction.canceled += unCrouch;
        sprintAction.performed += Sprint;
        shootAction.performed += ShootEvent;
        reloadAction.performed += ReloadEvent;
        //grenadeAction.performed += OnGrenadeThrow;
        grenadeAction.Enable();
        joinAction.Enable();
        pauseAction.performed += OnPause;
        aimAction.Enable();
        adsLeftAction.Enable();
        adsRightAction.Enable();
        swapWeaponsContAction.performed += WeaponSelectController;
        swapWeaponsScrollAction.performed += WeaponSelectMouse;
        interactAction.Enable();
        scoreboardAction.Enable();
    }

    private void OnDisable()
    {
        //movementAction.performed -= OnMovement;
        //movementAction.canceled -= OnMovementStop;
        movementAction.Disable();
        lookAction.Disable();
        //jumpAction.performed -= OnJumpStart;
        jumpAction.Disable();
        crouchAction.performed -= Crouch;
        crouchAction.canceled -= unCrouch;
        sprintAction.performed -= Sprint;
        shootAction.performed -= ShootEvent;
        reloadAction.performed -= ReloadEvent;
        //grenadeAction.performed -= OnGrenadeThrow;
        grenadeAction.Disable();
        joinAction.Disable();
        pauseAction.performed -= OnPause;
        aimAction.Disable();
        adsLeftAction.Disable();
        adsRightAction.Disable();
        swapWeaponsContAction.performed -= WeaponSelectController;
        swapWeaponsScrollAction.performed -= WeaponSelectMouse;
        interactAction.Disable();
        scoreboardAction.Disable();
    }



    void Movement()
    {
        agentSpeedVert = MovementInput.y; //This is needed for animations add it to serialized field
        agentSpeedHori = MovementInput.x;

        if (characterController.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        Vector3 inputDirection = new Vector3(MovementInput.x, 0f, MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection); //Transforms direction from local to world space
        worldDirection.Normalize();
        playerVel.x = worldDirection.x * speed;
        playerVel.z = worldDirection.z * speed;

        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * shootDist, Color.green);

        if (JumpTriggered && jumpCount < 1 && !isCrouching)
        {
            anim.SetTrigger("Jump");
            jumpCount++;
            playerVel.y = jumpForce;
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }

        characterController.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

    }

    void Sprint(InputAction.CallbackContext context)
    {
        if (!isSliding)
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

    //void HandleJumping() //Was going to be the jump action but couldn't figure it out
    //{



    //    //if (characterController.isGrounded)
    //    //{

    //    //    playerVel.y = -0.5f;
    //    //    if (JumpTriggered || Input.GetButton("Jump")) ;
    //    //    {
    //    //        anim.SetTrigger("Jump");
    //    //        jumpCount++;
    //    //        playerVel.y = jumpForce;
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    playerVel.y -= gravity * Time.deltaTime;
    //    //}
    //}

    //--------------Crouch event methods-------------//
    void Crouch(InputAction.CallbackContext context)
    {
        if (!isCrouching)
        {
            characterController.height = crouchLevel;
            speed = crouchSpeed;
            isCrouching = true;
            canSlide = true;
            if (isSprinting && !isSliding)
            {
                StartCoroutine(Slide2());
            }

        }
        //else
        //{
        //    characterController.height = origHeight;
        //    speed += crouchMod;
        //    isCrouching = false;
        //}

    }
    void unCrouch(InputAction.CallbackContext context)
    {
        if (isCrouching)
        {
            characterController.height = origHeight;
            speed = origSpeed;
            isCrouching = false;
            canSlide = false;
        }
    }
    //------------------------------------------------//

    IEnumerator Slide2()
    {
        isSliding = true;

        if (!slideSoundPlayed)
        {
            aud.PlayOneShot(audSlide, audSlideVol);
            slideSoundPlayed = true;
        }

        if (!slideAnimPlayed)
        {
            anim.SetTrigger("Slide");
            slideAnimPlayed = true;
        }

        isCrouching = true;
        isSprinting = false;

        // Increase the player's speed for the slide
        characterController.height = crouchLevel;
        speed = slideSpeed;

        //    playerVel.x = transform.forward.x * slideSpeed;
        //    playerVel.z = transform.forward.z * slideSpeed;

        //    characterController.Move(playerVel * Time.deltaTime);
        //    playerVel.y -= gravity * Time.deltaTime;

        // Maintain the slide for the specified duration
        yield return new WaitForSeconds(slideLockout);

        // After sliding, stop the slide and return to normal movement
        isSliding = false;
        //playerVel = Vector3.zero;
        speed = origSpeed;
        isSliding = false;

    }

    //void Slide()
    //{
    //    isSliding = true;
    //    if (canSlide)
    //    {

    //        if (!slideSoundPlayed)
    //        {
    //            aud.PlayOneShot(audSlide, audSlideVol);
    //            slideSoundPlayed = true;
    //        }

    //        if (!slideAnimPlayed)
    //        {
    //            anim.SetTrigger("Slide");
    //            slideAnimPlayed = true;
    //        }

    //        isCrouching = true;
    //        isSprinting = false;

    //        if (characterController.isGrounded)
    //        {
    //            jumpCount = 0;
    //            playerVel = Vector3.zero;
    //        }
    //        characterController.height = crouchLevel;

    //        playerVel.x = transform.forward.x * slideSpeed;
    //        playerVel.z = transform.forward.z * slideSpeed;

    //        characterController.Move(playerVel * Time.deltaTime);
    //        playerVel.y -= gravity * Time.deltaTime;

    //        if (slideLockout > 0)
    //        {
    //            slideLockout -= Time.deltaTime;
    //        }
    //        else if (slideLockout <= 0)
    //        {
    //            if (!isCrouching)
    //                characterController.height = origHeight;

    //            //slideSoundPlayed = false;
    //            //slideAnimPlayed = false;
    //            slideLockout = slideLockoutTime;


    //            playerVel = Vector3.zero;
    //            speed = origSpeed;
    //            isSliding = false;
    //            StartCoroutine(SlideCooldown());

    //        }

    //    }
    //}

    IEnumerator SlideCooldown()
    {
        canSlide = false;
        yield return new WaitForSeconds(slideCooldown);
        canSlide = true;
    }

    void Rotation()
    {
        float mouseYInput = invertYAxis ? -LookInput.y : LookInput.y;
        float mouseXRotation = LookInput.x * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);
        //verticalRotation -= inputHandler.LookInput.y * mouseSensitivity;
        verticalRotation -= mouseYInput * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }


    public void UpdatePlayerUI()
    {
        playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    //public void UpdateAmmoUI()
    //{
    //    GameManager.instance.playerAmmoBar.fillAmount = (float)weaponList[selectedWeapon].currentAmmo / weaponList[selectedWeapon].startAmmo;

    //}

    //void OnPauseInput(InputAction.CallbackContext obj)
    //{


    //    if (menuActive == null)
    //    {
    //        actionMapName = "UI";
    //        player = inputAsset.FindActionMap(actionMapName);
    //        //GameManager.instance.StatePause();
    //        menuActive = menuPause;
    //        eventSystem.firstSelectedGameObject = firstSelectedButtonInPause;
    //        menuActive.SetActive(true);
    //    }
    //    else if (menuActive == menuPause)
    //    {
    //        //GameManager.instance.StateUnpause();
    //        actionMapName = "Player";
    //        player = inputAsset.FindActionMap(actionMapName);
    //        menuActive.SetActive(false);
    //        menuActive = null;

    //    }

    //}

    void ShootEvent(InputAction.CallbackContext context)
    {
        if (!isShooting && !isReloading)
        {
            StartCoroutine(Shoot());
        }
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
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, shootDist, ~ignoreMask))
                {
                    //Debug.Log(hit.collider.name);

                    IDamage dmg = hit.collider.GetComponent<IDamage>();

                    if (hit.transform != transform && dmg != null)
                    {
                        dmg.takeDamage(shootDamage);
                        //If hit's gameObject is inside the deadTracker, then update Player Kill Count and display
                        if (GameManager.instance.CallTheDead(hit.collider.name))
                        {
                            GameManager.instance.statsTracker[this.gameObject.name].updateKills();
                            GameManager.instance.statsTracker[this.gameObject.name].updateKDR();
                            GameManager.instance.DisplayKillMessage(gameObject, hit.collider.gameObject);
                          //  Debug.Log(GameManager.instance.statsTracker[this.gameObject.name].getAllStats());
                        }
                    }
                    else
                    {
                        Instantiate(weaponList[selectedWeapon].hitEffect, hit.point, Quaternion.identity);
                    }
                }

                //Debug.Log("Rotating arm");
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
            //Switch weapons when shuriken is out of ammo
            if (weaponList[selectedWeapon].type != WeaponStats.WeaponType.projectile)
            {
                StartCoroutine(Reload());
            }
            else
            {
                if (weaponList.Count > 1)
                {
                    selectedWeapon++;

                    if (selectedWeapon > weaponList.Count - 1)
                        selectedWeapon = 0;

                    WeaponChange();
                    UpdateAmmoUI();
                }

            }
        }

        isShooting = false;
    }

    void ReloadEvent(InputAction.CallbackContext context)
    {
        if (!isReloading && !isShooting)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (weaponList[selectedWeapon].currentAmmo < weaponList[selectedWeapon].startAmmo)
        {
            Debug.Log("reload");
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

    public void UpdateAmmoUI()
    {
        GameManager.instance.playerAmmoBar.fillAmount = (float)weaponList[selectedWeapon].currentAmmo / weaponList[selectedWeapon].startAmmo;
    }

    public void updateGrenadeUI()
    {
        GameManager.instance.grenadeStack.text = (rubberBallCount).ToString();
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

    void WeaponSelectController(InputAction.CallbackContext context) //Making this to work with the controller
    {
        if (!isShooting)
        {
            if (weaponList.Count <= 1)
                return;

            selectedWeapon++;

            if (selectedWeapon > weaponList.Count - 1)
                selectedWeapon = 0;

            WeaponChange();
            UpdateAmmoUI();
        }
        
    }
    void WeaponSelectMouse(InputAction.CallbackContext context) //Making this to work with the 
    {
        if (!isShooting)
        {
            if (weaponList.Count <= 1)
                return;

            float scrollValue = context.ReadValue<Vector2>().y;

            if (scrollValue > 0)
            {
                selectedWeapon++;

                if (selectedWeapon > weaponList.Count - 1)
                    selectedWeapon = 0;


                WeaponChange();
            }
            else if (scrollValue < 0)
            {
                selectedWeapon--;

                if (selectedWeapon < 0)
                    selectedWeapon = weaponList.Count - 1;

                WeaponChange();
            }

            UpdateAmmoUI();
        }
            
    }

    //void OnGrenadeThrow(InputAction.CallbackContext context)
    //{
    //    Debug.Log("ThrowYoShit");
    //    if (!isShooting && !isReloading)
    //    {
    //        if (rubberBallCount > 0)
    //        {
    //            //StartCoroutine(ThrowItem());
    //            playerRig.weight = 0;
    //            anim.SetLayerWeight(8, 1f);
    //            GameManager.instance.playerScript.Munch(audRubberBall, audRubberBallVol);
    //            anim.SetTrigger("ThrowGrenade");
    //        }
    //    }
    //}

    //IEnumerator ThrowItem() ///GrenadeThrow
    //{
    //    WeaponToggleOff();
    //    yield return new WaitForSeconds(throwDelay);
    //    GameObject item = Instantiate(itemPrefab, itemSpawnPoint.transform.position, itemSpawnPoint.transform.rotation);
    //    Rigidbody rb = item.GetComponent<Rigidbody>();
    //    rb.velocity = playerCamera.transform.forward * throwForce;
    //    grenadeHUD.SetActive(false);
    //    rubberBallCount--;
    //    updateGrenadeUI();
    //    yield return new WaitForSeconds(throwDelay);
    //    WeaponToggleOn();
    //}

    void WeaponToggleOff()
    {
        isShooting = true;
        isReloading = true;
        weaponHUD.SetActive(false);
        grenadeHUD.SetActive(true);

    }

    void WeaponToggleOn()
    {
        weaponHUD.SetActive(true);
        isShooting = false;
        isReloading = false;
    }

    void OnPause(InputAction.CallbackContext context)
    {
        GameManager.instance.OnPause();
    }

    public int GetMaxBallCount()
    {
        return rubberBallMaxCount;
    }

    //-----------------------NOTICE: Keep these Methods at the Bottom please-------------------------------
    public void spawnPlayer()
    {
        HP = HPOrig;
        UpdatePlayerUI();
        characterController.enabled = false;
        transform.position = GameManager.instance.playerSpawn.transform.position;
        characterController.enabled = true;
        playerMeshRenderer.enabled = true;

        gameObject.GetComponent<CapsuleCollider>().enabled = true;
        deathCamera.gameObject.SetActive(false);

        isDead = false;

    }

    public void respawnPlayer()
    {
        UpdatePlayerUI();
        characterController.enabled = false;
        transform.position = GameManager.instance.playerSpawn.transform.position;
        characterController.enabled = true;
    }

    //ITarget Specific Methods
    //public GameObject declareOBJ(GameObject obj)
    //{
    //    return gameObject;
    //}

    //public bool declareDeath()
    //{
    //    if (HP <= 0)
    //    {
    //        //Increment death by 1 in GameManager's statsTracker dictionary
    //        return true;
    //    }
    //    else
    //        return false;
    //}

    //END ITarget Methods

    //FOR DONUT KING
    public void DKPickedUp()
    {
        HP = HPOrig;
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

    IEnumerator flashScreenDamage()
    {
        GameManager.instance.damageFlash.SetActive(true);
        playerMeshRenderer.material.color = dmgColor;
        yield return new WaitForSeconds(0.2f);
        playerMeshRenderer.material.color = origColor;
        GameManager.instance.damageFlash.SetActive(false);
    }

    //IDamage Method
    //public void takeDamage(int amount)
    //{
    //    if (!isDead)
    //    {
    //        HP -= amount;
    //        StartCoroutine(flashScreenDamage());
    //        UpdatePlayerUI();
    //        aud.PlayOneShot(audDamage[Random.Range(0, audDamage.Length)], audDamageVol);

    //        if (HP <= 0 && GameManager.currentMode == GameManager.gameMode.NIGHTSHIFT)
    //        {
    //            GameManager.instance.YouLose();
    //        }
    //        else if (HP <= 0 && GameManager.currentMode == GameManager.gameMode.DONUTKING2)
    //        {
    //            if (GameManager.instance.statsTracker[name].getDKStatus() == true)
    //                GameManager.instance.dropTheDonut(this.gameObject);
    //            //Camera.main.gameObject.SetActive(false);
    //            isDead = true;
    //            GameManager.instance.DeclareSelfDead(gameObject);
    //            gameObject.GetComponent<CapsuleCollider>().enabled = false;
    //            playerMeshRenderer.enabled = false;
    //            deathCamera.gameObject.SetActive(true);
    //            //while (GameManager.instance.statsTracker[name] > 0)              
    //        }
    //    }
    //}
    
    //-------------------Animation methods-------------------
    public void AnimThrowWeapon()
    {
        weaponModel.SetActive(false);
        Instantiate(shurikenProjectile, shurikenSpawnPoint.transform.position, shurikenSpawnPoint.transform.rotation);
    }
    public void AnimThrowDone()
    {
        throwAnimDone = true;
        anim.SetLayerWeight(8, 0);
        weaponModel.SetActive(true);
        isShooting = false;
    }
    //---------------------------------------------------------

    public void Munch(AudioClip clip, float vol)
    {
        aud.PlayOneShot(clip, vol);
    }

    //-----------------GameManager and DonutPickup-------------
    public void ToggleMyLight()
    {
        if (DKLight.activeSelf == false)
            DKLight.SetActive(true);
        else
            DKLight.SetActive(false);
    }

    public void ResetPlayer()
    {
        HP = HPOrig;
        UpdatePlayerUI();
        if (isDead)
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
            playerMeshRenderer.enabled = true;
            deathCamera.gameObject.SetActive(false);
            isDead = false;
        }
    }
    //---------------------------------------------------------
}
