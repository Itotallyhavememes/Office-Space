using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEditor.Rendering;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.ProBuilder.MeshOperations;


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    private int maxBots = 4;
    private int maxPlayers = 4;
    private int botCount;
    public List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] List<LayerMask> playerLayers;

    [Header("Match UI")]
    [SerializeField] GameObject matchSettingsFirst;
    [SerializeField] List<GameObject> playerIcons;
    [SerializeField] List<Text> playerTogglesText;
    [SerializeField] GameObject botsDropdown;
    [SerializeField] Slider timerSlider;
    [SerializeField] GameObject roundsDropdown;
    [SerializeField] GameObject threePlayerBlockout;
    [SerializeField] GameObject globalScoreBoard;
    [SerializeField] GameObject TwoPlayBlock1;
    [SerializeField] GameObject TwoPlayBlock2;
    [SerializeField] TMP_Text MatchSetMsg;

    private TMP_Dropdown botsDropdownValue;
    
    List<Color> playerToggleColors = new List<Color> { Color.red, Color.blue, Color.yellow, Color.green };
    private Color activePlayerTxtColor = Color.white;
    private Color activeBotColor = Color.gray;
    private Color defaultBackgroundColor;
    private Color defaultTextColor;
    private string defaultTogglesText = "N/A";

    [Header("Start Match")]
    [SerializeField] GameObject matchSettingsMenu;
    [SerializeField] GameObject[] enemyPrefabs;
    public bool matchStarted;

    private PlayerInputManager playerInputManager;

    [Header("Player HP")]
    public int HP;

    private void Awake()
    {
        instance = this;

        playerInputManager = FindObjectOfType<PlayerInputManager>();
        defaultBackgroundColor = playerIcons[0].GetComponent<Image>().color;
        defaultTextColor = playerTogglesText[0].color;

        botsDropdownValue = botsDropdown.GetComponent<TMP_Dropdown>();
        matchStarted = false;
        
    }

    private void Start()
    {
        //GameManager.instance.StatePause();
        GameManager.instance.ActivateMenu(matchSettingsMenu);
        GameManager.instance.isMultiplayer = true;
        MatchSetMsg.text = string.Empty;
        //GameManager.instance.AddToTracker(this.gameObject);
        //EventSystem.current.SetSelectedGameObject(matchSettingsFirst);
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    void AddPlayer(PlayerInput player)
    {
        players.Add(player);
        //cameras.Add(player.GetComponentInChildren<Camera>());
        GameManager.instance.playerSpawn.transform.position = spawnPoints[players.Count - 1].position;
        GameManager.instance.playerSpawn.transform.rotation = spawnPoints[players.Count - 1].rotation;
        player.GetComponent<ControllerTest>().spawnPlayer();
        player.GetComponent<ControllerTest>().ResetPlayer();
        player.name = "Player " + players.Count.ToString();
        maxBots--;

        if (players.Count > 1)
            player.GetComponent<AudioListener>().enabled = false;

        while (players.Count + botCount > maxPlayers)
        {
            botCount--;
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < maxPlayers; i++)
        {
            playerIcons[i].GetComponent<Image>().color = defaultBackgroundColor;
            playerTogglesText[i].text = defaultTogglesText;
            playerTogglesText[i].color = defaultTextColor;
        }

        for (int i = 0; i < players.Count; i++)
        {
            playerIcons[i].GetComponent<Image>().color = playerToggleColors[i];
            playerTogglesText[i].text = "P" + (i + 1).ToString();
            playerTogglesText[i].color = activePlayerTxtColor;
        }

        for (int j = players.Count; j < players.Count + botCount; j++) //Bots toggle
        {
            playerIcons[j].GetComponent<Image>().color = activeBotColor;
            playerTogglesText[j].text = "CPU";
            playerTogglesText[j].color = activePlayerTxtColor;
        }

        botsDropdownValue.value = botCount;
        GameManager.instance.SetBotCount(botCount);
    }

    public void ToggleActiveBots(int botSetting)
    {
        if (botSetting > maxBots)
        {
            botCount = maxBots;
        }
        else
        {
            botCount = botSetting;
        }

        RefreshUI();
    }

    public void BotSpawner()
    {
        for (int i = 0; i < botCount; i++)
        {
            //int rand = Random.Range(0, enemyPrefabs.Length);
            GameObject enemy = enemyPrefabs[i];
            Instantiate(enemy, spawnPoints[i + players.Count].position, spawnPoints[i + players.Count].rotation);
            //enemy.SetActive(true);
        }
        //GameManager.instance.InstantiateGlobalScoreBoard();
    }

    public void StartMatch()
    {
        if (players.Count > 0 && players.Count + botCount > 1)
        {
            playerInputManager.DisableJoining();
            BotSpawner();
            matchSettingsMenu.SetActive(false);
            ReactivatePlayerBlockouts();
            GameManager.instance.CurrRound++; //Makes this 1
            for (int i = 0; i < players.Count; i++)
            {
                players[i].GetComponent<Animator>().enabled = true;
                players[i].GetComponent<ControllerTest>().deathCamera.rect
                    = players[i].GetComponent<ControllerTest>().playerCamera.rect;
                //players[i].GetComponent<ControllerTest>().weaponCamera.rect
                //    = players[i].GetComponent<ControllerTest>().playerCamera.rect;
                //players[i].GetComponent<ControllerTest>().UI.SetActive(true);
                //ACTIVATE SHOP UI
                //players[i].GetComponent<ControllerTest>().StartCoroutine(players[i].GetComponent<ControllerTest>().DisableControlsTimer());
                players[i].GetComponent<ControllerTest>().ActivateShopUI();
                players[i].GetComponent<ControllerTest>().multEventSystem.playerRoot = players[i].GetComponent<ControllerTest>().localUI;
            }

            if (players.Count == 2)
            {
                players[0].GetComponent<ControllerTest>().playerCamera.rect = new Rect(0.25f, 0.5f, 0.5f, 0.5f);
                players[0].GetComponent<ControllerTest>().deathCamera.rect
                    = players[0].GetComponent<ControllerTest>().playerCamera.rect;
                players[0].GetComponent<ControllerTest>().weaponCamera.rect
                    = players[0].GetComponent<ControllerTest>().playerCamera.rect;

                players[1].GetComponent<ControllerTest>().playerCamera.rect = new Rect(0.25f, 0, 0.5f, 0.5f);
                players[1].GetComponent<ControllerTest>().deathCamera.rect
                    = players[1].GetComponent<ControllerTest>().playerCamera.rect;
                players[1].GetComponent<ControllerTest>().weaponCamera.rect 
                    = players[1].GetComponent<ControllerTest>().playerCamera.rect;
            }

            matchStarted = true;
            GameManager.instance.SetDKTimer((int)timerSlider.value * 60);
            AssignLayers();
            //GameManager.instance.SetDKTimer(15);
        }
        else
        {
            MatchSetMsg.text = string.Empty;
            GameManager.instance.aud.PlayOneShot(GameManager.instance.wrongButton, GameManager.instance.audwrongButtonVol);
            if (players.Count == 0)
            {
                MatchSetMsg.text = "MINIMUM 1 REAL PLAYER";
                MatchSetMsg.color = Color.red;
            }
            else if (players.Count + botCount == 1)
            {
                MatchSetMsg.text = "NEED 1 BOT OR 2 REAL PLAYERS";
                MatchSetMsg.color = Color.red;
            }
        }
    }

    void AssignLayers()
    {
        //gameObject.layer uses only integers, but we can turn a layer name into a layer integer using LayerMask.NameToLayer()

        for(int i = 0; i < players.Count; i++)
        {
            int playerLayer = LayerMask.NameToLayer("Player" + (i + 1).ToString());
            players[i].GetComponent<ControllerTest>().weaponModel.layer = playerLayer;

            // Switch on layer 14, leave others as-is
            players[i].GetComponent<ControllerTest>().weaponCamera.cullingMask |= (1 << playerLayer);
        }
        
    }

    public void ResetPlayerRoots()
    {
        foreach (var player in players)
        {
            player.GetComponent<ControllerTest>().multEventSystem.playerRoot = player.GetComponent<ControllerTest>().localUI;
        }
    }

    public void DeactivatePlayerCameras()
    {
        foreach (var player in players)
        {
            player.GetComponent<ControllerTest>().playerCamera.gameObject.SetActive(false);
            player.GetComponent<ControllerTest>().deathCamera.gameObject.SetActive(false);
        }
    }
    public void ReactivatePlayerCameras()
    {
        foreach (var player in players)
        {
            player.GetComponent<ControllerTest>().playerCamera.gameObject.SetActive(false);
        }
    }

    public void DeactivatePlayerBlockouts()
    {
        if (players.Count == 2)
        {
            TwoPlayBlock1.SetActive(false);
            TwoPlayBlock2.SetActive(false);
        }
        else if (players.Count == 3)
        {
            threePlayerBlockout.SetActive(false);
            globalScoreBoard.SetActive(false);
        }

    }
    
    public void ReactivatePlayerBlockouts()
    {
        if (players.Count == 2)
        {
            TwoPlayBlock1.SetActive(true);
            TwoPlayBlock2.SetActive(true);
        }
        else if (players.Count == 3)
        {
            threePlayerBlockout.SetActive(true);
            globalScoreBoard.SetActive(true);
        }
    }

    public void DeactivateWeaponCamera()
    {
        foreach(var player in players)
        {
            player.GetComponent<ControllerTest>().weaponCamera.gameObject.SetActive(false);
        }
    }
    
    public void ReactivateWeaponCamera()
    {
        foreach (var player in players)
        {
            player.GetComponent<ControllerTest>().weaponCamera.gameObject.SetActive(true);
        }
    }
}
