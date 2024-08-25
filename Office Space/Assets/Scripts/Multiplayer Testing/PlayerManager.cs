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
    [SerializeField] GameObject playerBlockout;

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
            Instantiate(enemy, spawnPoints[i].position, spawnPoints[i].rotation);
        }

    }

    public void StartMatch()
    {
        if (players.Count > 0 && players.Count + botCount > 1)
        {
            playerInputManager.DisableJoining();
            BotSpawner();
            matchSettingsMenu.SetActive(false);
            if(players.Count == 3)
                playerBlockout.SetActive(true);

            for (int i = 0; i < players.Count; i++)
            {
                players[i].GetComponent<Animator>().enabled = true;
                players[i].GetComponent<ControllerTest>().deathCamera.rect
                    = players[i].GetComponent<ControllerTest>().playerCamera.rect;
                players[i].GetComponent<ControllerTest>().UI.SetActive(true);
                //ACTIVATE SHOP UI
                players[i].GetComponent<ControllerTest>().ActivateShopUI();
            }

            matchStarted = true;
            GameManager.instance.SetDKTimer((int)timerSlider.value * 60);
            //GameManager.instance.StateUnpause();


        }
    }


}
