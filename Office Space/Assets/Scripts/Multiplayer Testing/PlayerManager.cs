using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEditor.Rendering;
using Unity.VisualScripting;
using System.Linq;


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] List<LayerMask> playerLayers;
    public List<Camera> cameras;

    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        if (cameras.Count > 0)
        {
            cameras.Clear();
        }
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void Update()
    {
        
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
        cameras.Add(player.GetComponentInChildren<Camera>());
        GameManager.instance.playerSpawn.transform.position = spawnPoints[players.Count - 1].position;

        player.GetComponent<PlayerControl>().spawnPlayer();

        Debug.Log(Input.GetJoystickNames().First());

        if (players.Count == 2)
        {
            cameras[0].rect.Set(0, 0.5f, 1, 0.5f);
            cameras[1].rect.Set(0, 0, 1, 0.5f);
            Debug.Log("There's 2 players on screen");
        }

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
