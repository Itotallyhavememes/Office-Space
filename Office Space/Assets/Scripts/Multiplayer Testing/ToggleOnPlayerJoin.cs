using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleOnPlayerJoin : MonoBehaviour
{
    PlayerInputManager playerInputManager;

    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += ToggleCamera;
    }
    private void OnDisable()
    {
        playerInputManager.onPlayerJoined += ToggleCamera;
    }

    void ToggleCamera(PlayerInput joinInput)
    {
        this.gameObject.SetActive(false);
    }
}
