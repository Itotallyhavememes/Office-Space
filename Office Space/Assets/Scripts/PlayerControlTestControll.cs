using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;


public class PlayerControlTestControll : MonoBehaviour
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

    [Header("Player UI")]
    [SerializeField] MultiplayerEventSystem eventSystem;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject firstSelectedButtonInPause;
    public Image playerHPBar;
    public Image playerAmmoBar;


    //[Header("----- Weapons -----")]
    //[SerializeField] WeaponStats starterWeapon;
    //[SerializeField] public List<WeaponStats> weaponList;
    //[SerializeField] GameObject weaponModel;
    //[SerializeField] float shootRate;
    //[SerializeField] int shootDamage;
    //[SerializeField] float reloadTime;
    //int selectedWeapon;

    [Header("Look Sensitivity")]
    [SerializeField] float mouseSensitivity = 2.0f;
    [SerializeField] float upDownRange = 90.0f;

    [SerializeField] Camera playerCam;

    private CharacterController characterController;
    private Vector3 currentMovement;
    private float verticalRotation;

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
    //InputAction splitCameraAction; //testing

    //auto-implemented property with a get and set accessor. Can be read from anywhere (public), but can only be set from within the class (private)
    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public float SprintValue { get; private set; }
    public bool ShootTriggered { get; private set; }
    public bool ReloadTriggered { get; private set; }
    public bool GrenadeTriggered { get; private set; }
    public bool CycleWeaponTriggered { get; private set; }
    public bool SplitCameraTriggered { get; private set; }

    private void Awake()
    {

        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap(actionMapName);

        //allows us to reference the action and find them  
        movementAction = player.FindAction(movement);
        lookAction = player.FindAction(look);
        jumpAction = player.FindAction(jump);
        sprintAction = player.FindAction(sprint);
        shootAction = player.FindAction(shoot);
        reloadAction = player.FindAction(reload);
        grenadeAction = player.FindAction(grenade);
        cycleWeaponAction = player.FindAction(cycleWeapon);
        joinAction = player.FindAction(join);
        pauseAction = player.FindAction(pause);
        pauseAction = player.FindAction(pause);
        resumeAction = inputAsset.FindActionMap("UI").FindAction(resume);
        //splitCameraAction = player.FindAction(splitCamera); //testing

        RegisterInputActions();

        InputSystem.settings.defaultDeadzoneMin = leftStickDeadzoneValue;
        characterController = GetComponent<CharacterController>();

        eventSystem = FindObjectOfType<MultiplayerEventSystem>();
    }
    private void Start()
    {
        HPOrig = HP;
    }

    private void Update()
    {
        HandleMovement();
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

        sprintAction.performed += context => SprintValue = context.ReadValue<float>();
        sprintAction.canceled += context => SprintValue = 0f;

        shootAction.performed += context => ShootTriggered = true;
        shootAction.canceled += context => ShootTriggered = false;

        reloadAction.performed += context => ReloadTriggered = true;
        reloadAction.canceled += context => ReloadTriggered = false;

        grenadeAction.performed += context => GrenadeTriggered = true;
        grenadeAction.canceled += context => GrenadeTriggered = false;

        //cycleWeaponAction.performed += context => CycleWeaponTriggered = true;
        //cycleWeaponAction.canceled += context => CycleWeaponTriggered = false;
    }

    //On Enable and Disable is required as this input manager uses an event handler
    private void OnEnable()
    {
        movementAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
        shootAction.Enable();
        reloadAction.Enable();
        grenadeAction.Enable();
        //cycleWeaponAction.Enable();
        joinAction.Enable();
        pauseAction.started += OnPauseInput;
        resumeAction.started += OnPauseInput;
    }

    private void OnDisable()
    {
        movementAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
        shootAction.Disable();
        reloadAction.Disable();
        grenadeAction.Disable();
        //cycleWeaponAction.Disable();
        joinAction.Disable();
        //splitCameraAction.started -= CameraSplit;
        pauseAction.started -= OnPauseInput;
        resumeAction.started -= OnPauseInput;
    }



    void HandleMovement()
    {
        //If it is above the value, then you are sprinting so multiply by the modifier, otherwise multiply by 1
        float speed = walkSpeed * (SprintValue > 0 ? sprintModifier : 1f);

        Vector3 inputDirection = new Vector3(MovementInput.x, 0f, MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection); //Transforms direction from local to world space
        worldDirection.Normalize();

        currentMovement.x = worldDirection.x * speed;
        currentMovement.z = worldDirection.z * speed;
        HandleJumping();
        characterController.Move(currentMovement * Time.deltaTime);
    }

    void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;
            if (JumpTriggered)
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y -= gravity * Time.deltaTime;
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
}

