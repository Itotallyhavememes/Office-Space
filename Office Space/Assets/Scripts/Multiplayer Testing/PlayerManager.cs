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
    [SerializeField] List<GameObject> playerIcons;
    [SerializeField] List<Text> playerTogglesText;
    [SerializeField] GameObject botsDropdown;
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
    [SerializeField] bool isMultiplayer;
    private int singlePlayerCount;


    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        instance = this;

        if (isMultiplayer)
        {
            playerInputManager = FindObjectOfType<PlayerInputManager>();
        }
        else
        {
            GameManager.instance.player.GetComponent<CharacterController>().enabled = false;
            singlePlayerCount = 1;
        }

        defaultBackgroundColor = playerIcons[0].GetComponent<Image>().color;
        defaultTextColor = playerTogglesText[0].color;

        botsDropdownValue = botsDropdown.GetComponent<TMP_Dropdown>();
        matchStarted = false;

        RefreshUI();
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
        if (!matchStarted)
        {
            players.Add(player);
            //cameras.Add(player.GetComponentInChildren<Camera>());
            GameManager.instance.playerSpawn.transform.position = spawnPoints[players.Count - 1].position;

            player.GetComponent<ControllerTest>().spawnPlayer();
            player.GetComponent<ControllerTest>().enabled = false; //Needs to change to new controller
            player.name = "Player " + players.Count.ToString();
            maxBots--;

            while (players.Count + botCount > maxPlayers)
            {
                botCount--;
            }

            RefreshUI();

        }

        //Debug.Log(Input.GetJoystickNames().First());

        //if (players.Count == 2)
        //{
        //    cameras[0].rect.Set(0, 0.5f, 1, 0.5f);
        //    cameras[1].rect.Set(0, 0, 1, 0.5f);
        //    Debug.Log("There's 2 players on screen");
        //}

        //// converts layer mask (bit) to an int
        //int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);

        ////set the layer
        //playerParent.GetComponentInChildren<CinemachineFreeLook>().gameObject.layer = layerToAdd;
        ////add the layer
        //playerParent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd; //bitwise operation
        ////set the action in the custom Cinemachine Input Handler
        //playerParent.GetComponentInChildren<InputHandler>().horizontal = player.actions.FindAction("Look");
    }

    public void RefreshUI()
    {
        for (int i = 0; i < maxPlayers; i++)
        {
            playerIcons[i].GetComponent<Image>().color = defaultBackgroundColor;
            playerTogglesText[i].text = defaultTogglesText;
            playerTogglesText[i].color = defaultTextColor;
        }

        if (isMultiplayer)
        {
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
        }
        else //singlePlayer
        {
            playerIcons[0].GetComponent<Image>().color = playerToggleColors[0];
            playerTogglesText[0].text = "P" + (0 + 1).ToString();
            playerTogglesText[0].color = activePlayerTxtColor;

            for (int j = singlePlayerCount; j < players.Count + botCount; j++) //Bots toggle
            {
                playerIcons[j].GetComponent<Image>().color = activeBotColor;
                playerTogglesText[j].text = "CPU";
                playerTogglesText[j].color = activePlayerTxtColor;
            }
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
        for (int i = players.Count; i < players.Count + botCount; i++)
        {
            int rand = Random.Range(0, enemyPrefabs.Length);
            GameObject enemy = enemyPrefabs[rand];
            Instantiate(enemy, spawnPoints[i].position, spawnPoints[i].rotation);
            //enemy.transform.position = new Vector3(transform.position.x + transform.forward.x, transform.position.y, transform.position.z);
        }

    }

    public void StartMatch()
    {
        if (players.Count > 0 && players.Count + botCount > 1)
        {
            BotSpawner();

            for (int i = 0; i < players.Count; i++)
            {
                players[i].GetComponent<ControllerTest>().enabled = true;
                players[i].GetComponent<Animator>().enabled = true;

            }

            matchSettingsMenu.SetActive(false);
            matchStarted = true;
        }
    }

    public void StartSinglePlayer()
    {
        BotSpawner();
        GameManager.instance.player.GetComponent<CharacterController>().enabled = true;
        matchSettingsMenu.SetActive(false);
        matchStarted = true;
    }

}
