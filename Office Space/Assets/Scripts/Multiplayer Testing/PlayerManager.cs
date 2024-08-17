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


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] List<LayerMask> playerLayers;
    //public List<Camera> cameras;

    [SerializeField] List<Toggle> playerToggles;
    [SerializeField] List<GameObject> playerIcons;
    [SerializeField] List<Text> playerTogglesText;
    List<Color> playerToggleColors = new List<Color> { Color.red, Color.blue, Color.yellow, Color.green };

    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        //if (cameras.Count > 0)
        //{
        //    cameras.Clear();
        //}
        playerInputManager = FindObjectOfType<PlayerInputManager>();
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

        player.GetComponent<PlayerControl>().spawnPlayer();

        playerIcons[players.Count - 1].GetComponent<Image>().color = playerToggleColors[players.Count - 1];
        //playerToggles[players.Count - 1].colors.selectedColor.Equals(Color.red);
        //playerToggles[players.Count - 1].Select();
        playerTogglesText[players.Count - 1].text = "P" + players.Count.ToString();
        playerTogglesText[players.Count - 1].color = Color.white;

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
}
