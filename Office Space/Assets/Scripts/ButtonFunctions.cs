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

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.StateUnpause();
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
        GameManager.instance.ReturnToPrevUI();
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
