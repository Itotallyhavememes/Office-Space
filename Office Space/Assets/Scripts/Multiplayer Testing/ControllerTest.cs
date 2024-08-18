using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class ControllerTest : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] InputActionAsset inputAsset;


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
    [SerializeField] string cycleWeapon = "Cycle Weapon";
    [SerializeField] string join = "Join";
    [SerializeField] string pause = "Pause";
    [SerializeField] string resume = "Cancel";
    [SerializeField] string crouch = "Crouch";
    [SerializeField] string aim = "Aim";
    [SerializeField] string adsLeft = "ADS Left";
    [SerializeField] string adsRight = "ADS Right";
    [SerializeField] string swapWeapons = "Swap Weapon";
    [SerializeField] string interact = "Interact";
    [SerializeField] string scoreboard = "Scroeboard";

    [Header("Deadzone Values")]
    [SerializeField] float leftStickDeadzoneValue;

    [Header("Movement Speeds")]
    [SerializeField] float walkSpeed = 3.0f;
    [SerializeField] float sprintModifier = 2.0f;

    [Header("Camera Settings")]
    [SerializeField] GameObject cameraLockOn;
    [SerializeField] bool invertYAxis = false;

    [Header("Jump Parameters")]
    [SerializeField] float jumpForce = 8.0f;
    [SerializeField] float gravity = 22.0f;

    [Header("HP Parameters")]
    public int HP;
    private int HPOrig;

    [Header("Height Parameters")]
    float OriginHeight;
    [SerializeField] float crouchLevel;
    [SerializeField] float CrouchMod;
    bool isCrouching;

    [Header("Player UI")]
    [SerializeField] MultiplayerEventSystem eventSystem;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject firstSelectedButtonInPause;
    public Image playerHPBar;
    public Image playerAmmoBar;


    [Header("----- Weapons -----")]
    [SerializeField] WeaponStats starterWeapon;
    [SerializeField] public List<WeaponStats> weaponList;
    [SerializeField] GameObject weaponModel;
    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] float reloadTime;
    [SerializeField] float shootDist;
    public bool isReloading;
    public bool isShooting;

    int selectedWeapon;
    [SerializeField] float raycastRotationRecoil;
    [SerializeField] float raycastRotationReload;

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


    [Header("----- Shuriken -----")]
    //Shuriken Variables
    [SerializeField] GameObject shurikenSpawnPoint;
    [SerializeField] GameObject shurikenProjectile;

    [Header("Look Sensitivity")]
    [SerializeField] float mouseSensitivity = 2.0f;
    [SerializeField] float upDownRange = 90.0f;

    [SerializeField] Camera playerCam;

    private CharacterController characterController;
    private Vector3 currentMovement;
    private float verticalRotation;


    //added to match playerControl
    int jumpCount;
    [SerializeField] int slideSpeed;
    float slideLockout;
    bool slideSoundPlayed;
    bool isSliding;
    [SerializeField] float slideLockoutTime;
    bool canSlide;
    [SerializeField] float slideCooldown;

    float originSpeed;
    float crouchSpeed;

    [SerializeField] Animator anim;
    [SerializeField] AudioSource aud;

    [SerializeField] LayerMask ignoreMask;


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
    InputAction swapWeaponsAction;
    InputAction interactAction;
    InputAction scoreboardAction;
    //InputAction splitCameraAction; //testing

    //auto-implemented property with a get and set accessor. Can be read from anywhere (public), but can only be set from within the class (private)
    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool SprintTrigger { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool CrouchTrigger { get; private set; }
    public float SprintValue { get; private set; }
    public bool ShootTriggered { get; private set; }
    public bool ReloadTriggered { get; private set; }
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
        cycleWeaponAction = player.FindAction(cycleWeapon);
        joinAction = player.FindAction(join);
        pauseAction = player.FindAction(pause);
        pauseAction = player.FindAction(pause);
        resumeAction = inputAsset.FindActionMap("UI").FindAction(resume);
        aimAction = player.FindAction(aim);
        adsLeftAction = player.FindAction(adsLeft);
        adsRightAction = player.FindAction(adsRight);
        swapWeaponsAction = player.FindAction(swapWeapons);
        interactAction = player.FindAction(interact);
        scoreboardAction = player.FindAction(scoreboard);
        //splitCameraAction = player.FindAction(splitCamera); //testing

        RegisterInputActions();

        InputSystem.settings.defaultDeadzoneMin = leftStickDeadzoneValue;
        characterController = GetComponent<CharacterController>();

        eventSystem = FindObjectOfType<MultiplayerEventSystem>();
    }
    private void Start()
    {
        HPOrig = HP;
        OriginHeight = characterController.height;
        originSpeed = walkSpeed;
        canSlide = true;
        slideLockout = slideLockoutTime;
        crouchSpeed = walkSpeed - CrouchMod;
        GetWeaponStats(starterWeapon);
        //Add self to gameManager's bodyTracker
        GameManager.instance.AddToTracker(this.gameObject);
        // Call spawnPlayer
    }

    private void Update()
    {
        HandleMovement();
       // Vector3 inputDirection = new Vector3(MovementInput.x, 0f, MovementInput.y);
       // Vector3 worldDirection = transform.TransformDirection(inputDirection);
        HandleRotation();
        UpdatePlayerUI();    
    }
    
    //void PrintDevices()
    //{
    //    foreach (var devices in InputSystem.devices)
    //    {

    //    }
    //}

    //Registers each action and assigns them a value
    void RegisterInputActions()
    {
        movementAction.performed += context => MovementInput = context.ReadValue<Vector2>();
        movementAction.canceled += context => MovementInput = Vector2.zero;

        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        jumpAction.performed += context => JumpTriggered = true;
        jumpAction.canceled += context => JumpTriggered = false;

        crouchAction.performed += context => CrouchTrigger = true;
        crouchAction.canceled += context => CrouchTrigger = false;

        sprintAction.performed += context => SprintValue = context.ReadValue<float>();
        sprintAction.canceled += context => SprintValue = 0f;

        sprintAction.performed += context => SprintTrigger = true;
        sprintAction.canceled += context => SprintTrigger = false;

        shootAction.performed += context => ShootTriggered = true;
        shootAction.canceled += context => ShootTriggered = false;

        reloadAction.performed += context => ReloadTriggered = true;
        reloadAction.canceled += context => ReloadTriggered = false;

        grenadeAction.performed += context => GrenadeTriggered = true;
        grenadeAction.canceled += context => GrenadeTriggered = false;

        aimAction.performed += context => AimTriggered = true;
        aimAction.canceled += context => AimTriggered = false;

        adsLeftAction.performed += context => AdsLeftTriggered = true;
        adsLeftAction.canceled += context => AdsLeftTriggered = false;

        adsRightAction.performed += context => adsRightTriggered = true;
        adsRightAction.canceled += context => adsRightTriggered = false;

        swapWeaponsAction.performed += context => SwapWeaponsTriggered = true;
        swapWeaponsAction.canceled += context => SwapWeaponsTriggered = false;

        interactAction.performed += context => InteractTriggered = true;
        interactAction.canceled += context => InteractTriggered = false;

        scoreboardAction.performed += context => ScoreboardTriggered = true;
        scoreboardAction.canceled += context => ScoreboardTriggered = false;
        //cycleWeaponAction.performed += context => CycleWeaponTriggered = true;
        //cycleWeaponAction.canceled += context => CycleWeaponTriggered = false;
    }

    //On Enable and Disable is required as this input manager uses an event handler
    private void OnEnable()
    {
        movementAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        crouchAction.Enable();
        sprintAction.Enable();
        shootAction.Enable();
        reloadAction.Enable();
        grenadeAction.Enable();
        //cycleWeaponAction.Enable();
        joinAction.Enable();
        pauseAction.started += OnPauseInput;
        resumeAction.started += OnPauseInput;
        aimAction.Enable();
        adsLeftAction.Enable();
        adsRightAction.Enable();
        swapWeaponsAction.Enable();
        interactAction.Enable();
        scoreboardAction.Enable();
    }

    private void OnDisable()
    {
        movementAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        crouchAction.Disable();
        sprintAction.Disable();
        shootAction.Disable();
        reloadAction.Disable();
        grenadeAction.Disable();
        //cycleWeaponAction.Disable();
        joinAction.Disable();
        //splitCameraAction.started -= CameraSplit;
        pauseAction.started -= OnPauseInput;
        resumeAction.started -= OnPauseInput;
        aimAction.Disable();
        adsLeftAction.Disable();
        adsRightAction.Disable();
        swapWeaponsAction.Disable();
        interactAction.Disable();
        scoreboardAction.Disable();
    }

 

    void HandleMovement()
    {
        if (characterController.isGrounded)
        {
            jumpCount = 0;
            currentMovement = Vector3.zero;
        }
        //If it is above the value, then you are sprinting so multiply by the modifier, otherwise multiply by 1
        float speed = walkSpeed * (SprintValue > 0 ? sprintModifier : 1f);

        Vector3 inputDirection = new Vector3(MovementInput.x, 0f, MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection); //Transforms direction from local to world space
        worldDirection.Normalize();
        currentMovement.x = worldDirection.x * speed;
        currentMovement.z = worldDirection.z * speed;

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.green);


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

        if (JumpTriggered && jumpCount < 1 && !isCrouching)
        {

   

           // anim.SetTrigger("Jump");
            jumpCount++;
            currentMovement.y = jumpForce;
           // aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);


        }
       // Debug.Log("not jumping");
        characterController.Move(currentMovement * Time.deltaTime);
        currentMovement.y -= gravity * Time.deltaTime;

        if (!isShooting && !isReloading)
        {

            if (ShootTriggered)
            {
               
                StartCoroutine(Shoot());
               
            }
            Debug.Log("between methods");
            if (ReloadTriggered)
            {
                Debug.Log("before reload");
                StartCoroutine(Reload());
                Debug.Log("after reload");
            }



        }
        if (characterController.isGrounded)
        {
            HandleCrouch();
        }
       // HandleShoot();
        HandlePlayerInteraction();
        
    }

    void HandleJumping()
    {
       

        
        //if (characterController.isGrounded)
        //{

        //    currentMovement.y = -0.5f;
        //    if (JumpTriggered || Input.GetButton("Jump")) ;
        //    {
        //        anim.SetTrigger("Jump");
        //        jumpCount++;
        //        currentMovement.y = jumpForce;
        //    }
        //}
        //else
        //{
        //    currentMovement.y -= gravity * Time.deltaTime;
        //}
    }

    void HandleCrouch()
    {

    

        if (CrouchTrigger)
        {
            characterController.height = crouchLevel;
            walkSpeed = crouchSpeed;
            isCrouching = true;
            if(JumpTriggered)
            {
                isSliding = true;
                
            }
            if(isSliding)
            {
                
                slide();
                
            }
        }
        else if (isCrouching && (!CrouchTrigger))       
        {
            characterController.height = OriginHeight;            
            walkSpeed = originSpeed;           
            isCrouching = false;
        } 
        
    }
    void slide()
    {
       

        if (canSlide)
        {

            Debug.Log("started");

            //if (!slideSoundPlayed)
            //{
            //    aud.PlayOneShot(audSlide, audSlideVol);
            //    slideSoundPlayed = true;
            //}

            //if (!slideAnimPlayed)
            //{
            //    anim.SetTrigger("Slide");
            //    slideAnimPlayed = true;
            //}

            isCrouching = true;
            //SprintTrigger = false;
            if (characterController.isGrounded)
            {
                jumpCount = 0;
                currentMovement = Vector3.zero;
            }
            characterController.height = crouchLevel;
            
            currentMovement.x = transform.forward.x * slideSpeed;
            currentMovement.z = transform.forward.z * slideSpeed;
            
            characterController.Move(currentMovement * Time.deltaTime);
            currentMovement.y -= gravity * Time.deltaTime;
            
            if (slideLockout > 0)
            {
                slideLockout -= Time.deltaTime;
            }
            else if (slideLockout <= 0)
            {
                Debug.Log("no errors");
                if (!CrouchTrigger)
                    characterController.height = OriginHeight;

                //slideSoundPlayed = false;
                //slideAnimPlayed = false;
                slideLockout = slideLockoutTime;
                
                
                currentMovement = Vector3.zero;
                walkSpeed = originSpeed;
                StartCoroutine(SlideCooldown());
                Debug.Log("sliding");
               
            }
           
        }
    }

    IEnumerator SlideCooldown()
    {
        Debug.Log("incorutine");
        canSlide = false;
        yield return new WaitForSeconds(slideCooldown);
        canSlide = true;
        isSliding = false;
    }
    void HandleShoot()
    {
        
        
            if (AimTriggered)
            {
                Debug.Log("Aimming");
            }

            if (AimTriggered && adsRightTriggered)
            {

                Debug.Log("ADS Right");
            }

            if (AimTriggered && AdsLeftTriggered)
            {

                Debug.Log("ADS Left");
            }

            if (SwapWeaponsTriggered)
            {
 

            }


           

            if (ReloadTriggered)
            {
           
                Debug.Log("reloading");
            }

            if (GrenadeTriggered)
            {

                Debug.Log("Throwing Grenade");
            }

        

    }
    void HandlePlayerInteraction()
    {
        if (InteractTriggered)
        {
            Debug.Log("interact");
        }

        if (ScoreboardTriggered)
        {

            Debug.Log("scoreboard");
        }
    }



    void HandleRotation()
    {
        
        float mouseYInput = invertYAxis ? -LookInput.y : LookInput.y;
        float mouseXRotation = LookInput.x * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);
        //verticalRotation -= inputHandler.LookInput.y * mouseSensitivity;
        verticalRotation -= mouseYInput * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        cameraLockOn.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }


    public void UpdatePlayerUI()
    {
        playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    //public void UpdateAmmoUI()
    //{
    //    GameManager.instance.playerAmmoBar.fillAmount = (float)weaponList[selectedWeapon].currentAmmo / weaponList[selectedWeapon].startAmmo;

    //}

    void OnPauseInput(InputAction.CallbackContext obj)
    {


        if (menuActive == null)
        {
            actionMapName = "UI";
            player = inputAsset.FindActionMap(actionMapName);
            //GameManager.instance.StatePause();
            menuActive = menuPause;
            eventSystem.firstSelectedGameObject = firstSelectedButtonInPause;
            menuActive.SetActive(true);
        }
        else if (menuActive == menuPause)
        {
            //GameManager.instance.StateUnpause();
            actionMapName = "Player";
            player = inputAsset.FindActionMap(actionMapName);
            menuActive.SetActive(false);
            menuActive = null;

        }

    }
    IEnumerator Shoot()
    {
        isShooting = true;
        Debug.Log("shooting");
        if (weaponList[selectedWeapon].currentAmmo > 0)
        {
            weaponList[selectedWeapon].currentAmmo--;
            UpdateAmmoUI();

            if (weaponList[selectedWeapon].type == WeaponStats.WeaponType.raycast)
            {
                // aud.PlayOneShot(audHandFire, audHandFireVol);
               
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
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
                            Debug.Log(GameManager.instance.statsTracker[this.gameObject.name].getAllStats());
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
                Debug.Log("Shoot");
            }
        }
        else if (!isReloading && (weaponList[selectedWeapon].currentAmmo <= 0))
        {
            StartCoroutine(Reload());
        }

        isShooting = false;
        Debug.Log("Done shooting");
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
                //aud.PlayOneShot(audHandReloadBegin, audHandReloadBeginVol);
               
                yield return new WaitForSeconds(reloadTime);
                weaponModel.transform.Rotate(Vector3.right * raycastRotationReload);
               // aud.PlayOneShot(audHandReloadEnd, audHandReloadEndVol);
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
}
