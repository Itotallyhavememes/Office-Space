using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
        SceneManager.LoadScene("Donut King 2.0");
    }
    public void startNightShift()
    {
        SceneManager.LoadScene("Corporate Espionage");
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
    public void settings()
    {
        GameManager.instance.OpenSettings();
    }

    public void Controls()
    {
        GameManager.instance.OpenControls();
    }

    public void returnButton()
    {
        GameManager.instance.ReturnFromSettings();
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
