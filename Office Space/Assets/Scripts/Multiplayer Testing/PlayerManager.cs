using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;


public class PlayerManager : MonoBehaviour
{
    private List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] List<LayerMask> playerLayers;

    private PlayerInputManager playerInputManager;

    private void Awake()
    {
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

        GameManager.instance.playerSpawn.transform.position = spawnPoints[players.Count - 1].position;

        player.GetComponent<PlayerControl>().spawnPlayer();

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
