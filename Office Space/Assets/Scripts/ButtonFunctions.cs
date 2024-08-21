using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;



public class ButtonFunctions : MonoBehaviour
{
    // Start is called before the first frame update
    public void resume()
    {
        GameManager.instance.StateUnpause();
    }

    public void startDoughnutKing()
    {
        GameManager.currentMode = GameManager.gameMode.DONUTKING2;
        GameManager.instance.ActivateObjectiveScreen();
    }

    public void openDKScene()
    {
        SceneManager.LoadScene("Donut King 2.0");
        GameManager.instance.StateUnpause();
    }
    public void startNightShift()
    {
        GameManager.currentMode = GameManager.gameMode.NIGHTSHIFT;
        GameManager.instance.ActivateObjectiveScreen();
    }

    public void openNSScene()
    {
        SceneManager.LoadScene("Corporate Espionage");
        GameManager.instance.StateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        DonutPickUp.totalDonuts = 0;
        GameManager.instance.StateUnpause();
    }
  
    public void gameModes()
    {
        GameManager.instance.OpenGameModes();
    }

    public void createMatch()
    {
        GameManager.instance.OpenCreateMatch();
    }
    
    public void loadWageCage()
    {
        SceneManager.LoadScene("Donut King 3");
        GameManager.instance.StateUnpause();
        //GameManager.currentMode = GameManager.gameMode.DONUTKING2;
    }

    public void startMatch()
    {
        PlayerManager.instance.StartMatch();
    }

    public void settings()
    {
        GameManager.instance.OpenSettings();
    }

    public void Controls()
    {
        GameManager.instance.OpenControls();
    }
    public void creditsMenu()
    {
        GameManager.instance.OpenCredits();
    }

    public void returnButton()
    {
        GameManager.instance.ReturnFromSettings();
    }

    public void returnToTittle()
    {
        SceneManager.LoadScene("Title");

        GameManager.currentMode = GameManager.gameMode.TITLE;
        if (GameManager.instance.isPaused)
            GameManager.instance.StateUnpause();

    }
    
    public void returnToMainFromTitle()
    {
        GameManager.instance.ReturnToMainFromTitle();
    }

    public void retryAmountButtonConfirm()
    {
        GameManager.instance.respawn = true;
    }

    public void retryAmountButtonCancel()
    {
        GameManager.instance.YouLose();
    }
    public void respawn()
    {

        GameManager.instance.playerScript.respawnPlayer();
        GameManager.instance.StateUnpause();
    }
    public void startGame()
    {
        GameManager.instance.StateUnpause();
        GameManager.instance.ReturnFromSettings();
    }
    public void returnToTittleMenu()
    {
        GameManager.instance.ReturnToTittle();
    }

    public void openDK3Objective()
    {
        GameManager.instance.ActivateObjectiveScreen();
        GameManager.instance.StatePause();
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
     Application.Quit();

#endif
    }

}
