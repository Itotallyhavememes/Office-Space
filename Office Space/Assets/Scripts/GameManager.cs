using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum gameMode { DONUTKING2, NIGHTSHIFT }
    public static gameMode currentMode;
    //Dictionary to hold player and NE_enemies along with live/dead stats

    [SerializeField] List<GameObject> bodyTracker;
    [SerializeField] List<string> deadTracker;
    [SerializeField] List<GameObject> spawnPoints;
    public Dictionary<string, int> donutCountList;
    [SerializeField] gameMode modeSelection;
    [SerializeField] int timerTime;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuScore;
    [SerializeField] GameObject scoreDisplay;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuTitle;
    [SerializeField] GameObject menuGameModes;
    [SerializeField] GameObject menuSettings;
    [SerializeField] GameObject menuControls;
    [SerializeField] TMP_Text donutCountText;
    [SerializeField] GameObject timerUI;
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text donutCountUI;

    [SerializeField] TMP_Text scoreBoardPlacementsText;
    [SerializeField] TMP_Text scoreBoardNamesText;
    [SerializeField] TMP_Text scoreBoardScoreText;
    [SerializeField] TMP_Text activeScoreNamesText;
    [SerializeField] TMP_Text activeScoreText;

    // JOHN CODE FOR CHECKPOINT
    public GameObject playerSpawn;
    public GameObject checkPointPos;
    //
    public TMP_Text grenadeStack;
    public Transform PriorityPoint;

    public Image playerHPBar;
    public Image playerAmmoBar;

    public GameObject damageFlash;
    public GameObject player;
    public PlayerControl playerScript;
    public AudioMixer audioMixer;

    public bool isPaused;
    bool gameEnd;
    public int playerHPStart;
    public int playerAmmo;


    int playerDonutCount;
    public int enemyCount;
    public int Thresh;
    GameObject previousScreen;

    public string PlayerName;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerControl>();

        currentMode = modeSelection;

        Thresh = 29;
        bodyTracker = new List<GameObject>();
        donutCountList = new Dictionary<string, int>();
        // CHECK POINT
        playerSpawn = GameObject.FindWithTag("Player Spawn Pos");

        //
        //Transferring Donut UI to here

    }

    void Start()
    {
        if (currentMode == gameMode.DONUTKING2)
        {
            timerUI.SetActive(true);
            StartCoroutine(Timer());
        }
        playerHPStart = playerScript.HP;

    }

    //Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            else if (menuActive == menuPause)
            {
                StateUnpause();
            }
        }

        if (currentMode == gameMode.DONUTKING2 && !isPaused)
        {
            DisplayInfoScreen();
        }
    }

    IEnumerator EndGame()
    {

        yield return null;
    }

    IEnumerator Timer()
    {
        int timeElapsed = timerTime;
        int timerMinutes;
        int timerSeconds;
        string timeText = "";

        while (timeElapsed > 0)
        {
            if (!isPaused)
            {
                timerMinutes = timeElapsed / 60;
                timerSeconds = timeElapsed % 60;

                if (timerMinutes == 0 || timerMinutes < 10)
                    timeText = "0";

                timeText += timerMinutes.ToString() + ":";

                if (timerSeconds == 0 || timerSeconds < 10)
                    timeText += "0";

                timeText += timerSeconds.ToString();

                timerText.text = timeText;

                yield return new WaitForSeconds(1);

                --timeElapsed;
            }
        }
        timerText.text = "00:00";
        gameEnd = true;
        DonutKingGoal();
    }

    //START: enemy&player Tracker Methods
    //public method to have instantiated objects pass themselves
    public void AddToTracker(GameObject self)
    {
        bool canAdd = true; ;
        for (int i = 0; i < bodyTracker.Count; i++)
        {
            if (self.GetHashCode() == bodyTracker[i].GetHashCode())
            {
                canAdd = false;
                break;
            }
        }
        if (canAdd)
        {
            bodyTracker.Add(self);
            donutCountList.Add(self.name, 0);
        }
    }
    //Method for spawner. Also remove object from bodyTracker as new one will be instantiated upon spawn
    public void DeclareSelfDead(GameObject self, string type)
    {
        //for (int i = 0; i < bodyTracker.Count; i++)
        //{
        //    if(self.GetHashCode() == bodyTracker[i].GetHashCode())
        //        bodyTracker.Remove(bodyTracker[i]);
        //}
        deadTracker.Add(type);
    }
    //Method called in enemySpawner that returns the first entry in deadTracker
    public string SpawnTheDead()
    {
        //if (deadTracker.Count > 0)
        //{
        string returnable = deadTracker[0];
        deadTracker.RemoveAt(0);
        return returnable;
        //}

    }
    //method to search through tracker and return object
    public GameObject ReturnEntity(GameObject target)
    {
        for (int i = 0; i < bodyTracker.Count; i++)
        {
            if (target.GetHashCode() == bodyTracker[i].GetHashCode())
                return bodyTracker[i];
        }
        return null;
    }

    public void UpdateDonutCount(GameObject donutCollector)
    {
        donutCountList[donutCollector.name] += 1;

        //For Debugging purposes:
        foreach (KeyValuePair<string, int> pair in donutCountList)
        {
            Debug.Log(pair.Key.ToString() + " HAS: " + pair.Value.ToString());
        }
        donutCountText.text = donutCountList[player.name].ToString();

    }

    public void DonutDeclarationDay(GameObject donutOBJ)
    {
        if (PriorityPoint == null)
            PriorityPoint = donutOBJ.transform;
    }

    //public string GetPlayerDC(GameObject target)
    //{
    //    return donutCountList[player.name].ToString();
    //}
    ////method to set status to dead or alive for specific entity
    //public void SetEntityState(GameObject target, bool state)
    //{
    //    foreach(KeyValuePair<GameObject,bool> pair in bodyTracker)
    //    {
    //        if(pair.Key.Equals(target))
    //            bodyTracker[pair.Key] = state;
    //    }
    //}
    //END: enemy&player Tracker Methods

    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused);
        menuActive = null;
    }


    public void UpdateGameGoal(int amount) //Donut king 1 goal
    {
        playerDonutCount += amount;
        if (playerDonutCount >= DonutPickUp.totalDonuts)
        {
            //you win!
            StatePause();
            menuActive = menuWin;
            menuActive.SetActive(isPaused);
        }
    }

    public void DonutKingGoal() //Donut King 2 Goal
    {
        if (gameEnd)
        {
            scoreDisplay.SetActive(false);
            StatePause();
            menuActive = menuScore;
            menuActive.SetActive(true);
            TallyScores();
        }
    }

    void TallyScores()
    {
        scoreBoardNamesText.text = string.Empty;
        scoreBoardScoreText.text = string.Empty;
        var scoreBoard = donutCountList.OrderByDescending(pair => pair.Value);
        foreach (var score in scoreBoard)
        {
            scoreBoardNamesText.text += score.Key + '\n';
            scoreBoardScoreText.text += score.Value.ToString() + '\n';
        }
    }

    public void TallyActiveScores()
    {
        activeScoreNamesText.text = string.Empty;
        activeScoreText.text = string.Empty;
        var scoreBoard = donutCountList.OrderByDescending(pair => pair.Value);
        foreach (var score in scoreBoard)
        {
            activeScoreNamesText.text += score.Key + '\n';
            activeScoreText.text += score.Value.ToString() + '\n';
        }
    }

    public void DisplayInfoScreen()
    {
        if (Input.GetButtonDown("Info"))
        {
            TallyActiveScores();
            scoreDisplay.SetActive(true);
        }
        else if (Input.GetButtonUp("Info"))
        {
            scoreDisplay.SetActive(false);
        }
    }

    public void YouLose()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void ActivateMenu(GameObject active)
    {
        menuActive = active;
        menuActive.SetActive(true);
    }

    public void OpenControls()
    {
        menuActive = menuControls;
        ActivateMenu(menuActive);
    }

    public void OpenSettings()
    {
        previousScreen = menuActive;
        menuActive = menuSettings;
        ActivateMenu(menuActive);
    }

    public void OpenGameModes()
    {
        previousScreen = menuActive;
        menuActive = menuGameModes;
        ActivateMenu(menuActive);
    }

    public void ReturnFromSettings()
    {
        menuActive.SetActive(false);
        menuActive = previousScreen;
    }


    public void SetVolume(float volume)
    {
        //Debug.Log(volume);
        audioMixer.SetFloat("MasterAudio", volume);
    }
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

}
