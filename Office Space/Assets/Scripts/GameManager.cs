using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    float time;

    public bool isPaused = false;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        time = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetButtonDown("Cancel"))
        {
            if(menuActive == null)
            {
                statePaused();
                menuActive = menuPause;
                menuPause.SetActive(isPaused);
            }
            else if(menuActive == menuPause)
            {
                stateUnpaused();
            }
        }
    }
    public void statePaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void stateUnpaused()
    {
        isPaused = !isPaused;
        Time.timeScale = time;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        menuActive.SetActive(isPaused);
        menuActive = null;
    }
}
