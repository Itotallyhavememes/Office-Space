using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ParticipantStats
{
    [SerializeField] string DisplayName;
    [SerializeField] int moneyTotal, RoundsWon;
    public int timeHeld;
    //Changed Kills and Deaths to double to allow KDR to show up to the 0.01 decimal place
    [SerializeField] double Kills, Deaths, KDR;
    [SerializeField] bool isDonutKing;

    public ParticipantStats instantiateStats()
    {
        ////DebugLog("Instantiating StatsTracker Values...");
        Kills = 0;
        Deaths = 0;
        timeHeld = 0;
        KDR = 0.0f;
        isDonutKing = false;
        moneyTotal = GameManager.instance.startingMoney;//starting money for players
        return this;
    }

    //Methods to adjust struct values
    public void setDisplayName(string displayName) { DisplayName = displayName; }

    public void updateKills() { ++Kills; }

    public void updateDeaths() {  ++Deaths; }

    public double getDeaths() { return Deaths; }

    public double getKills() { return Kills; }

    public void updateKDR() 
    {
        double divisDeath;
        divisDeath = Deaths;
        if (Deaths == 0.0f)
            divisDeath = 1;
        KDR = Kills / divisDeath; 
    }

    public void updateDKStatus() 
    { 
        isDonutKing = !isDonutKing;
        if (!GameManager.instance.isThereDonutKing)
            GameManager.instance.isThereDonutKing = true;
    }
    public bool getDKStatus()
    {
        if (isDonutKing)
            return true;
        else
            return false;
    }

    public void updateTimeHeld()
    {
        timeHeld++;
        depositMoney(GameManager.instance.moneyForTimeHeld);
    }

    public void updateRoundsWon() { ++RoundsWon; }

    public int getRoundsWon() { return RoundsWon; }

    public int getTimeHeld() { return timeHeld; }

    public void resetTimeHeld() { timeHeld = 0; }

    //public void updateScore(int score) { scorePoints += score; }

    public void depositMoney(int money) 
    {
        int depositAMT, timeHeldReward;
        depositAMT = money;
        timeHeldReward = timeHeld;
        if (timeHeld == 0)
            timeHeldReward = 1;
        moneyTotal += depositAMT * timeHeldReward;
    }

    public void withdrawMoney(int money) { moneyTotal -= money; }

    public int getMoneyTotal() { return moneyTotal; }

    public string getMoneyTotalString() { return moneyTotal.ToString(); }

    //METHOD FOR PRINTING STATS FOR SCOREBOARD
    public string GetScoreStats()
    {
        string scoreStats;
        scoreStats = " " + timeHeld.ToString() + "|";
        scoreStats += " R: " + RoundsWon.ToString() + "|";
        return scoreStats;
    }
}
