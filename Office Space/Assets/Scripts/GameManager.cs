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
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //Test Variable
    public int RoundsWon;
    public enum gameMode { DONUTKING2, NIGHTSHIFT, TITLE }
    public static gameMode currentMode;

    [Header("Main Menu First Selected Options")]
    [SerializeField] GameObject mainMenuFirst;
    [SerializeField] GameObject createMatchFirst;
    [SerializeField] GameObject objectiveFirst;
    [SerializeField] GameObject settingsFirst;
    [SerializeField] GameObject controlsFirst;
    [SerializeField] GameObject mapSelectScreenFirst;
    [SerializeField] GameObject pauseFirst;
    [SerializeField] GameObject inGameSettingsFirst;
    [SerializeField] GameObject nextRoundFirst;
    [SerializeField] GameObject gameEndFirst;

    [Header("Match Settings Selected Option")]
    [SerializeField] GameObject matchSettingsFirst;

    //Dictionary to hold player and NE_enemies along with live/dead stats
    //OPTIMIZED: Moving donutDropDistance and donutDropItem from PlayerControl & EnemyAI to here
    [SerializeField] int MaxPlayers;
    [SerializeField] int NumberOfRounds;
    [SerializeField] int donutDropDistance;
    public GameObject donutDropItem;
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
    [SerializeField] GameObject menuScore; //The UI for when a Round Ends & For when Game Ends
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
    [SerializeField] GameObject menuShop;

    [SerializeField] public GameObject menuRetryAmount;
    [SerializeField] TMP_Text donutCountText;
    [SerializeField] GameObject timerUI;
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text donutCountUI;
    [SerializeField] GameObject[] vendingMachines;

    [SerializeField] int respawnTime;

    [SerializeField] TMP_Text scoreBoardTitleText;
    [SerializeField] TMP_Text scoreBoardWLMessageText;
    [SerializeField] TMP_Text scoreBoardPlacementsText;
    [SerializeField] TMP_Text scoreBoardNamesText;
    [SerializeField] TMP_Text scoreBoardScoreText;
    [SerializeField] TMP_Text scoreBoardRWText;
    [SerializeField] TMP_Text scoreBoardResultText;
    [SerializeField] TMP_Text activeScoreNamesText;
    [SerializeField] TMP_Text activeScoreText;
    [SerializeField] GameObject RetryButton;
    [SerializeField] GameObject NextRoundButton;

    //TIM TEST CODE FOR DK
    public bool isThereDonutKing;
    public GameObject TheDonutKing;
    //CODE FOR PJ's SHOP
    [SerializeField] public int moneyForKills, moneyForTimeHeld, moneyForDonutKing;
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
    public ItemThrow playerThrowScript;
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

    bool isShopDisplayed;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerScript = player.GetComponent<PlayerControl>();
            playerThrowScript = player.GetComponent<ItemThrow>();
        }
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
        Debug.Log("Did we restart?");
    }

    void Start()
    {
        canVend = true;
        if (currentMode == gameMode.DONUTKING2)
        {
            //EventSystem.current.SetSelectedGameObject(matchSettingsFirst); //Match settings
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
        Debug.Log(bodyTracker.Count.ToString() + " PLAYERS LOADED");
        //if (currentMode == gameMode.DONUTKING2)
        //    InstantiateScoreBoard();



    }

    //Update is called once per frame
    void Update()
    {
        //if (currentMode == gameMode.DONUTKING2 && !isShopDisplayed)
        //{
        //    //PJ's shop code
        //    ActivateMenu(menuShop);
        //    StatePause();
        //    isShopDisplayed = true;
        //}
        if (currentMode == gameMode.DONUTKING2 && !isPaused)
            TallyActiveScores();

        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
                EventSystem.current.SetSelectedGameObject(pauseFirst);
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

    //INSTANTIATING SCOREBOARD PLACEMENTS BASED OFF NUMBER OF PLAYERS:
    public void InstantiateScoreBoard()
    {
        scoreBoardPlacementsText.text = string.Empty;
        for (int i = 0; i < statsTracker.Count; ++i)
        {
            scoreBoardPlacementsText.text += i + 1;
            switch (i)
            {
                case 0:
                    {
                        scoreBoardPlacementsText.text += "st";
                        break;
                    }
                case 1:
                    {
                        scoreBoardPlacementsText.text += "nd";
                        break;
                    }
                case 2:
                    {
                        scoreBoardPlacementsText.text += "rd";
                        break;
                    }
                default:
                    {
                        scoreBoardPlacementsText.text += "th";
                        break;
                    }
            }
            scoreBoardPlacementsText.text += '\n';
        }
        scoreBoardPlacementsText.color = Color.yellow;
    }
    //METHOD FOR RESETTING EVERYTHING WITHOUT RESETTING PARTICPANTSTATS
    public void ResetTheRound()
    {
        //Changed to reflect that menuScore IS ALSO for Round Over
        if (menuActive = menuScore)
            menuScore.SetActive(false);
        //CHANGE END
        StateUnpause();
        ////Pause game
        //isPaused = true;
        //int SpawnIndex = 0;
        //PlayerControl playerBody = null;
        //enemyAI enemyBody = null;
        if (isThereDonutKing)
            dropTheDonut(TheDonutKing);
        while (bodyTracker.Count > 0)
        {
            deadTracker.Add(bodyTracker[0]);
            bodyTracker.RemoveAt(0);
        }
        int deadTrackTemp = deadTracker.Count;
        while (deadTracker.Count > 0)
        {

            for (int i = 0; i < deadTrackTemp; i++)
            {
                if (deadTracker[0].GetHashCode() == player.GetHashCode())
                {
                    playerSpawn.transform.position = spawnPoints[i].transform.position;
                    playerScript.spawnPlayer();
                }
                else
                {
                    deadTracker[0].transform.position = spawnPoints[i].transform.position;
                    deadTracker[0].SetActive(true);
                }
                bodyTracker.Add(deadTracker[0]);
                deadTracker.RemoveAt(0); //pop front
            }
        }

        //Reset Positions and Health/Ammo of all Live Participants
        //for (int i = 0; i < bodyTracker.Count; ++i)
        //{
        //    playerBody = bodyTracker[i].GetComponent<PlayerControl>();
        //    if (playerBody != null)
        //    {
        //        playerBody.ResetPlayer();
        //        playerBody = null;
        //    }
        //    else
        //    {
        //        enemyBody = bodyTracker[i].GetComponent<enemyAI>();
        //        enemyBody.ResetHP();
        //    }
        //    bodyTracker[i].transform.position = spawnPoints[i].transform.position;
        //    SpawnIndex++;
        //}

        ////Reset Positions and Health/Ammo of all Dead Participants
        //if (SpawnIndex < 4)
        //{
        //    for (int i = SpawnIndex; i < deadTracker.Count; ++i)
        //    {
        //        playerBody = deadTracker[i].GetComponent<PlayerControl>();
        //        if (playerBody != null)
        //        {
        //            playerBody.ResetPlayer();
        //            playerBody = null;
        //        }
        //        else
        //        {
        //            enemyBody = deadTracker[i].GetComponent<enemyAI>();
        //            enemyBody.ResetHP();
        //            if (deadTracker[i].activeSelf == false)
        //                deadTracker[i].SetActive(true);
        //        }
        //    }
        //}
        ////Restart the Timer???
        StartCoroutine(Timer());
        ////Unpause the game
        //isPaused = false;

        //StopCoroutine(Timer());
        //StartCoroutine(Timer());
        //

        //PJ's shop code
        isShopDisplayed = false;
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
        if (statsTracker.ContainsKey(self.name))
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
        //float respawnerTime = statsTracker[deadTracker[0].name].getTimeHeld() / 2;
        //if (respawnerTime == 0)
        //    respawnerTime = respawnTime;
        yield return new WaitForSeconds(respawnTime);
        int spawnIndex = Random.Range(0, statsTracker.Count);
        if (deadTracker.Count > 0 && deadTracker[0].GetHashCode() == player.GetHashCode())
        {
            playerSpawn.transform.position = spawnPoints[spawnIndex].transform.position;
            playerScript.spawnPlayer();
        }
        else if (deadTracker.Count > 0 && deadTracker[0].GetHashCode() != player.GetHashCode())
        {
            deadTracker[0].transform.position = spawnPoints[spawnIndex].transform.position;
            deadTracker[0].SetActive(true);
        }
        if (deadTracker.Count > 0)
        {
            bodyTracker.Add(deadTracker[0]);
            deadTracker.RemoveAt(0); //pop front
        }

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
    //public void UpdateDonutCount(GameObject donutCollector, int amount)
    //{
    //        //statsTracker[donutCollector.name] += amount;

    //        ////For Debugging purposes:
    //        //foreach (KeyValuePair<string, int> pair in statsTracker)
    //        //{
    //        //    Debug.Log(pair.Key.ToString() + " HAS: " + pair.Value.ToString());
    //        //}
    //        //donutCountText.text = statsTracker[player.name].ToString();
    //}

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
            TallyFinalScores();
            menuActive = menuScore;
            menuActive.SetActive(true);
            EventSystem.current.SetSelectedGameObject(gameEndFirst);
        }
    }

    void TallyFinalScores()
    {
        scoreBoardNamesText.text = string.Empty;
        scoreBoardScoreText.text = string.Empty;
        scoreBoardWLMessageText.text = string.Empty;
        scoreBoardRWText.text = string.Empty;
        scoreBoardTitleText.text = string.Empty;
        scoreBoardResultText.text = string.Empty;
        var scoreBoard = statsTracker.OrderByDescending(pair => pair.Value.getTimeHeld());
        int scoreIndex = 0;
        string winnerName = null;
        string TheTrueKing = null;
        //Check for Winner HERE:
        winnerName = scoreBoard.ElementAt(0).Key;
        statsTracker[winnerName].updateRoundsWon();

        if (RetryButton.activeSelf == true)
            RetryButton.SetActive(false);
        if (NextRoundButton.activeSelf == true)
            NextRoundButton.SetActive(false);
        TheTrueKing = CheckTrueWinner();
        if (TheTrueKing != null)
        {
            //GAME OVER:
            //Make Changes to Text here For:
            //Change ScoreBoard Title to "ALL HAIL:"
            //Grab First Place Winner's:
            //-Name
            //-TimeHeld for Round
            //-Rounds Won
            //SetActive(true) for Win/Lose Message in relation to PLAYER
            //RESULT SUBMENU:
            //-Change Result Text to say GAME OVER
            //-SetActive(true) for RETRY
            scoreBoardTitleText.text = "ALL HAIL:";
            scoreBoardPlacementsText.text = "1st";
            scoreBoardNamesText.text = TheTrueKing;
            scoreBoardScoreText.text = statsTracker[TheTrueKing].getTimeHeld().ToString();
            scoreBoardRWText.text = statsTracker[TheTrueKing].getRoundsWon().ToString();
            if (player.name == TheTrueKing)
            {
                scoreBoardWLMessageText.text = "Enjoy Your Donut!";
                scoreBoardWLMessageText.color = Color.green;
            }
            else
            {
                scoreBoardWLMessageText.text = "No Donut For You :(";
                scoreBoardWLMessageText.color = Color.red;
            }
            RetryButton.SetActive(true);

            scoreBoardResultText.text = "GAME OVER";
        }
        else
        {
            //ROUND OVER:

            //Display:
            //All placements (applicable number)
            //All names
            //All TimeHeld for Round
            //All Rounds Won
            //keep SetActive(false) for Win/Lose message
            //RESULT SUBMENU:
            //-Change Result Text to say ROUND OVER
            //-SetActive(true) for NEXT ROUND
            InstantiateScoreBoard();
            scoreBoardTitleText.text = "TIME'S UP!";
            foreach (var score in scoreBoard)
            {
                //if (scoreIndex == 0)
                //{
                //    winnerName = score.Key;
                //    statsTracker[winnerName].updateRoundsWon();
                //}
                score.Value.resetTimeHeld();
                scoreBoardNamesText.text += score.Key + '\n';
                scoreBoardScoreText.text += score.Value.getTimeHeld().ToString() + '\n';
                scoreBoardRWText.text += score.Value.getRoundsWon().ToString() + '\n';
                scoreIndex++;
            }
            NextRoundButton.SetActive(true);
            EventSystem.current.SetSelectedGameObject(nextRoundFirst);
            scoreBoardResultText.text = "ROUND OVER";
        }
        //    menuActive = menuScore;
        //else if (!isThereTrueKing)
        //    menuActive = menuScore2;


        Debug.Log(winnerName + " : " + statsTracker[winnerName].getAllStats());
        ++RoundsWon;



    }

    string CheckTrueWinner()
    {
        foreach (var participant in statsTracker)
        {
            Debug.Log(participant.Key + " : " + participant.Value.getAllStats());
            if (participant.Value.getRoundsWon() == NumberOfRounds)
            {
                Debug.Log(participant.Key + " WINS THE GAME!");
                return participant.Key;
            }
        }
        return null;
    }

    public void TallyActiveScores()
    {
        activeScoreNamesText.text = string.Empty;
        activeScoreText.text = string.Empty;
        var scoreBoard = statsTracker.OrderByDescending(pair => pair.Value.getTimeHeld());
        foreach (var score in scoreBoard)
        {
            int timeElapsed = score.Value.timeHeld;
            int timerMinutes = timeElapsed / 60;
            int timerSeconds = timeElapsed % 60;
            string timeText = "";

            if (timerMinutes == 0 || timerMinutes < 10)
                timeText = "0";

            timeText += timerMinutes.ToString() + ":";

            if (timerSeconds == 0 || timerSeconds < 10)
                timeText += "0";

            timeText += timerSeconds.ToString();

            activeScoreNamesText.text += score.Key;
            if (score.Value.getDKStatus())
                activeScoreNamesText.text += " (K)";
            activeScoreNamesText.text += '\n';

            activeScoreText.text += "|" + timeText + '\n';
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
        if (currentMode == gameMode.DONUTKING2)
            EventSystem.current.SetSelectedGameObject(inGameSettingsFirst);
        else
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
        EventSystem.current.SetSelectedGameObject(pauseFirst);
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
        ActivateMenu(menuDK2Objective);
        EventSystem.current.SetSelectedGameObject(objectiveFirst);

    }

    //TIM ADDED METHODS FOR statsTracker
    //METHOD FROM: when enemy or player dies and needs to spawn a DONUT near them
    public void dropTheDonut(GameObject donutDropper)
    {
        //creates sphere that's the size of roamDist and selects a random position
        //Vector3 randDropPos = Random.insideUnitSphere * donutDropDistance;
        //randDropPos.y = donutDropItem.transform.position.y;
        ////Prevents getting null reference when creating random point
        //UnityEngine.AI.NavMeshHit hit;
        ////The "1" is in refernce to layer mask "1"
        //UnityEngine.AI.NavMesh.SamplePosition(randDropPos, out hit, donutDropDistance, 1);
        Debug.Log(donutDropper.name.ToString() + " DROPPED THE DONUT!");
        donutDropItem.transform.position = donutDropper.transform.position;
        if (donutDropItem.transform.position == donutDropper.transform.position)
            Debug.Log("YAHOO!");
        donutDropItem.SetActive(true);
        //Instantiate(donutDropItem, donutDropper.transform.position + randDropPos, donutDropItem.transform.rotation);

        //GameManager.instance.UpdateDonutCount(gameObject, -1);

        //Sets isDonutKing for current object to false, since initially was true
        //METHOD TO TOGGLE LIGHT SWITCH
        PlayerControl lightSwitchP = donutDropper.GetComponent<PlayerControl>();
        if (lightSwitchP != null)
            lightSwitchP.ToggleMyLight();
        else
        {
            enemyAI lightSwitchE = donutDropper.GetComponent<enemyAI>();
            lightSwitchE.ToggleMyLight();
            lightSwitchE.ToggleAmIKing();
            if (!lightSwitchE.getKingStatus())
                Debug.Log(donutDropper.name + " IS DETHRONED!!!");
        }
        statsTracker[donutDropper.name].updateDKStatus();
        PriorityPoint.Clear();
        PriorityPoint.Add(donutDropItem.transform);
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
        if (isThereDonutKing)
            isThereDonutKing = false;
        StopCoroutine(DKTimer());
        Debug.Log("Ending TIMER!");
    }

    public void DKLightSwitch(GameObject theDonutKing)
    {
        if (statsTracker[theDonutKing.name].GetHashCode() == player.GetHashCode())
        {
            playerScript.ToggleMyLight();
        }
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

    public void shopDoneButton()
    {
        Debug.Log("done");
        //menuActive.SetActive(false);
        StateUnpause();
    }

}
