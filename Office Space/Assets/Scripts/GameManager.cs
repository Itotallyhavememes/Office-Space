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
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum gameMode { DONUTKING2, NIGHTSHIFT, TITLE }
    public static gameMode currentMode;

    [Header("Main Menu First Selected Options")]
    [SerializeField] GameObject mainMenuFirst;
    [SerializeField] GameObject createMatchFirst;
    [SerializeField] GameObject settingsFirst;
    [SerializeField] GameObject controlsFirst;
    [SerializeField] GameObject mapSelectScreenFirst;

    [Header("Match Settings Selected Option")]
    [SerializeField] GameObject matchSettingsFirst;

    //Dictionary to hold player and NE_enemies along with live/dead stats
    //OPTIMIZED: Moving donutDropDistance and donutDropItem from PlayerControl & EnemyAI to here
    [SerializeField] int donutDropDistance;
    [SerializeField] GameObject donutDropItem;
    public List<GameObject> bodyTracker;
    public List<GameObject> deadTracker;
    public List<GameObject> spawnPoints;
    public List<string> CombatMessage;
    //Changed from donutCountList -> statsTracker
    //changed value from int -> ParticipantStats struct object
    public Dictionary<string, ParticipantStats> statsTracker;
    [SerializeField] gameMode modeSelection;

    [Header("DK Match Settings")]
    [SerializeField] int timerTime;
    [SerializeField] int botCount;
    [SerializeField] int roundsToPlay;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuScore;
    [SerializeField] GameObject scoreDisplay;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuTitle;
    [SerializeField] GameObject menuGameModes;
    [SerializeField] GameObject menuCreateMatch;
    [SerializeField] GameObject menuSettings;
    [SerializeField] GameObject menuControls;
    [SerializeField] GameObject menuCredits;
    [SerializeField] GameObject menuDK2Objective;
    [SerializeField] GameObject menuNSObjective;

    [SerializeField] public GameObject menuRetryAmount;
    [SerializeField] TMP_Text donutCountText;
    [SerializeField] GameObject timerUI;
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text donutCountUI;
    [SerializeField] GameObject[] vendingMachines;

    [SerializeField] int respawnTime;


    [SerializeField] TMP_Text scoreBoardPlacementsText;
    [SerializeField] TMP_Text scoreBoardNamesText;
    [SerializeField] TMP_Text scoreBoardScoreText;
    [SerializeField] TMP_Text scoreBoardResultText;
    [SerializeField] TMP_Text activeScoreNamesText;
    [SerializeField] TMP_Text activeScoreText;
    //TIM TEST CODE FOR DK
    public bool isThereDonutKing;
    public GameObject TheDonutKing;
    //CODE FOR PJ's SHOP
    [SerializeField] int moneyForKills, moneyForTimeHeld, moneyForDonutKing;
    // JOHN CODE FOR CHECKPOINT
    public GameObject playerSpawn;
    public GameObject checkPointPos;
    public int retryAmount;
    //
    public TMP_Text grenadeStack;
    public List<Transform> PriorityPoint;
    public Transform donutFocus;

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

    public bool respawn;

    public bool canVend;
    [SerializeField] float VendingCooldown;

    public int worldDonutCount;
    public int worldItemCount;

    Coroutine coroutine;


    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");

        if (player != null)
            playerScript = player.GetComponent<PlayerControl>();

        currentMode = modeSelection;

        Thresh = 29;
        bodyTracker = new List<GameObject>();
        statsTracker = new Dictionary<string, ParticipantStats>();
        PriorityPoint = new List<Transform>();
        CombatMessage = new List<string>();
        // CHECK POINT
        playerSpawn = GameObject.FindWithTag("Player Spawn Pos");
        respawn = false;
        //
        //TIM TEST CODE FOR DKSTATUS
        isThereDonutKing = false;

    }

    void Start()
    {
        canVend = true;
        if (currentMode == gameMode.DONUTKING2)
        {
            EventSystem.current.SetSelectedGameObject(matchSettingsFirst);
            timerUI.SetActive(true);
            RandomizeVending();
            StartCoroutine(Timer());
        }

        if (player != null)
            playerHPStart = playerScript.HP;
        else if (player == null)
            playerHPStart = 5;

        if (SceneManager.GetSceneByName("Title") == SceneManager.GetActiveScene())
        {
            StatePause();
            EventSystem.current.SetSelectedGameObject(mainMenuFirst);
        }
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

        if (deadTracker.Count > 0 && coroutine == null) //spawn logic
        {
            coroutine = StartCoroutine(SpawnTheDead());
        }
    }

    void RandomizeVending()
    {
        foreach (GameObject vending in vendingMachines)
        {
            vending.SetActive(false);
        }

        vendingMachines[Random.Range(0, vendingMachines.Length)].SetActive(true);
    }

    public void StartVendingMachineCooldown()
    {
        StartCoroutine(StartVendingCooldown());
    }

    IEnumerator StartVendingCooldown()
    {
        canVend = false;
        yield return new WaitForSeconds(VendingCooldown);
        canVend = true;
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
        bool canAdd = true;

        
        for (int i = 0; i < bodyTracker.Count; i++)
        {
            if (self.GetHashCode() == bodyTracker[i].GetHashCode()) ///Take it from here
            {
                canAdd = false;
                break;
            }
        }
        if(statsTracker.ContainsKey(self.name))
            canAdd = false;
        if (canAdd)
        { 
            bodyTracker.Add(self);
            //ParticipantStats::instantiateStats() returns ParticipantStats struct Object
            //Adds itself to statsTracker's Value field
            ParticipantStats objStat = new ParticipantStats();
            objStat = objStat.instantiateStats();
            statsTracker.Add(self.name, objStat);
            Debug.Log(self.name.ToString() + " : " + statsTracker[self.name].getAllStats());
            //BELOW: available code for when/if Beta requires username to be displayed, as opposed to Player or Enemy Types
            //statsTracker[self.name].setDisplayName(self.name);
        }
    }

    public void AddToSpawnList(GameObject spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
    }

    
    //Method for spawner. Also remove object from bodyTracker as new one will be instantiated upon spawn
    public void DeclareSelfDead(GameObject self)
    {
        for (int i = 0; i < bodyTracker.Count; i++)
        {
            if (self.GetHashCode() == bodyTracker[i].GetHashCode())
                bodyTracker.Remove(bodyTracker[i]);
        }
        deadTracker.Add(self);
        //Debug.Log(self.name + "DB: " + statsTracker[self.name].getDeaths().ToString());
        statsTracker[self.name].updateDeaths();
        statsTracker[self.name].updateKDR();
        Debug.Log(self.name.ToString() + statsTracker[self.name].getAllStats().ToString());
       // Debug.Log(self.name + "DA: " + statsTracker[self.name].getDeaths().ToString());
        //if (statsTracker[self.name].getDKStatus() == true)
        //    statsTracker[self.name].updateDKStatus();
        //if (statsTracker[self.name].getDeaths() > 0)
        //    Debug.Log(self.name.ToString() + " : " + statsTracker[self.name].getAllStats());
    }

    //public void CleanUpDictionary(GameObject self)
    //{
    //    if(donutCountList.ContainsKey(self.name))
    //        donutCountList.Remove(self.name);
    //}

    //Method called in enemySpawner that returns the first entry in deadTracker
    public IEnumerator SpawnTheDead()
    {
        yield return new WaitForSeconds(respawnTime);
        int spawnIndex = Random.Range(0, statsTracker.Count);
        if (deadTracker[0].GetHashCode() == player.GetHashCode())
        {
            playerSpawn.transform.position = spawnPoints[spawnIndex].transform.position;
            playerScript.spawnPlayer();
        }
        else
        {
            deadTracker[0].transform.position = spawnPoints[spawnIndex].transform.position;
            deadTracker[0].SetActive(true);
        }
        bodyTracker.Add(deadTracker[0]);
        deadTracker.RemoveAt(0); //pop front

        coroutine = null;
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

    public bool CallTheDead(string targetName)
    {
        for (int i = 0; i < deadTracker.Count; i++)
        {
            if (targetName == deadTracker[i].name)
                return true;
        }
        return false;
    }

    //CHANGE: Method from updating donut count number to Determining if participant is DonutKing
    //IF true: change priorityTarget in enemyAI to donutKing
    //IF false: stay course on current Donut location (singular)
    public void UpdateDonutCount(GameObject donutCollector, int amount)
    {
            //statsTracker[donutCollector.name] += amount;

            ////For Debugging purposes:
            //foreach (KeyValuePair<string, int> pair in statsTracker)
            //{
            //    Debug.Log(pair.Key.ToString() + " HAS: " + pair.Value.ToString());
            //}
            //donutCountText.text = statsTracker[player.name].ToString();
    }

    public void DonutDeclarationDay(GameObject donutOBJ)
    {
        if (GameManager.currentMode == gameMode.DONUTKING2)
        {
            PriorityPoint.Add(donutOBJ.transform);
            //PriorityPoint = donutOBJ.transform;
        }
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
            TallyFinalScores();
        }
    }

    void TallyFinalScores()
    {
        scoreBoardNamesText.text = string.Empty;
        scoreBoardScoreText.text = string.Empty;
        var scoreBoard = statsTracker.OrderByDescending(pair => pair.Value);
        int scoreIndex = 0;
        foreach (var score in scoreBoard)
        {
            if (scoreIndex == 0 && score.Key == "Player")
            {
                scoreBoardResultText.text = "Congrats! \nDonut King!";
                scoreBoardResultText.color = Color.green;
            }
            else if (scoreIndex == 0 && score.Key != "Player")
            {
                scoreBoardResultText.text = "If ya ain't First, You're LAST!";
                scoreBoardResultText.color = Color.red;
            }

            scoreBoardNamesText.text += score.Key + '\n';
            //scoreBoardScoreText.text += score.Value.ToString() + '\n';
            scoreIndex++;
        }
    }

    public void TallyActiveScores()
    {
        activeScoreNamesText.text = string.Empty;
        activeScoreText.text = string.Empty;
        var scoreBoard = statsTracker.OrderByDescending(pair => pair.Value);
        foreach (var score in scoreBoard)
        {
            activeScoreNamesText.text += score.Key + '\n';
            //activeScoreText.text += score.Value.ToString() + '\n';
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
        menuActive.SetActive(false);
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
        EventSystem.current.SetSelectedGameObject(controlsFirst);

    }

    public void OpenSettings()
    {
        previousScreen = menuActive;
        menuActive = menuSettings;
        ActivateMenu(menuActive);
        EventSystem.current.SetSelectedGameObject(settingsFirst);
    }

    public void OpenGameModes()
    {
        previousScreen = menuActive;
        menuActive = menuGameModes;
        ActivateMenu(menuActive);
    }

    public void OpenCreateMatch()
    {
        previousScreen = menuActive;        
        menuActive = menuCreateMatch;
        ActivateMenu(menuActive);
        EventSystem.current.SetSelectedGameObject(createMatchFirst);
    }

    public void OpenCredits()
    {
        menuActive = menuCredits;
        ActivateMenu(menuActive);
    }

    public void ReturnFromSettings()
    {
        menuActive.SetActive(false);
        menuActive = previousScreen;
    }

    public void ReturnToTittle() 
    {
        menuActive = menuTitle;
        ActivateMenu(menuActive);
    }
    
    public void ReturnToMainFromTitle()
    {
        menuActive.SetActive(false);
        menuActive = menuTitle;
        ActivateMenu(menuActive);
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
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

   public void GetNS_GoalScreen()
    {
        //scoreBoardScoreText.text = (statsTracker[player.name] * 10).ToString();
        //ActivateMenu(menuScore);
    }

    public void SetBotCount(int count)
    {
        botCount = count;
    }
    
    public void SetRounds(int rounds)
    {
        roundsToPlay = rounds;
    }

    public void SetDKTimer(int time) //Made public timer setter to access it from the Match Settings
    {
        timerTime = time; //Timer format is in seconds
    }

    public void ActivateObjectiveScreen()
    {
        if (currentMode == gameMode.DONUTKING2)
            ActivateMenu(menuDK2Objective);
        else if (currentMode == gameMode.NIGHTSHIFT)
            ActivateMenu(menuNSObjective);
        StatePause();

    }

    //TIM ADDED METHODS FOR statsTracker
    //METHOD FROM: when enemy or player dies and needs to spawn a DONUT near them
    public void dropTheDonut(GameObject donutDropper)
    {
        //creates sphere that's the size of roamDist and selects a random position
        Vector3 randDropPos = Random.insideUnitSphere * donutDropDistance;
        randDropPos.y = donutDropItem.transform.position.y;
        //Prevents getting null reference when creating random point
        UnityEngine.AI.NavMeshHit hit;
        //The "1" is in refernce to layer mask "1"
        UnityEngine.AI.NavMesh.SamplePosition(randDropPos, out hit, donutDropDistance, 1);
            donutDropItem.transform.position = donutDropper.transform.position + randDropPos;
            if (donutDropItem.transform.position == donutDropper.transform.position)
                Debug.Log("YAHOO!");
            donutDropItem.SetActive(true);
        //Instantiate(donutDropItem, donutDropper.transform.position + randDropPos, donutDropItem.transform.rotation);

        //GameManager.instance.UpdateDonutCount(gameObject, -1);
        Debug.Log(donutDropper.name.ToString() + " DROPPED THE DONUT!");
        //Sets isDonutKing for current object to false, since initially was true
        statsTracker[donutDropper.name].updateDKStatus();
        DownWithTheDonutKing();
        Debug.Log(donutDropper.name.ToString() + " : " + statsTracker[donutDropper.name].getAllStats());
    }

    public IEnumerator DKTimer()
    {
        //int timeElapsed = 0;
        
        while (isThereDonutKing && !isPaused)
        {
            Debug.Log(statsTracker[TheDonutKing.name].getTimeHeld().ToString());
            yield return new WaitForSeconds(1);
            statsTracker[TheDonutKing.name].updateTimeHeld();
            //++timeElapsed;
        }
    }
    public void DeclareTheDonutKing()
    {
        if (!isThereDonutKing)
            isThereDonutKing = true;
        Debug.Log("Starting TIMER!");
        StartCoroutine(DKTimer());
    }

    public void DownWithTheDonutKing()
    {
        if(isThereDonutKing)
            isThereDonutKing = false;
        StopCoroutine(DKTimer());
        Debug.Log("Ending TIMER!");
    }

    //METHOD CREATED TO ADD AND DISPLAY WHO KILLED WHO
    public void DisplayKillMessage(GameObject winner, GameObject defeated)
    {
        string cMessage;
        cMessage = winner.name.ToString() + " KILLED " + defeated.name.ToString();
        Debug.Log(cMessage);
        CombatMessage.Add(cMessage);
        if (CombatMessage.Count > 5)
            CombatMessage.RemoveAt(0);
    }
}
