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
        Debug.Log("Instantiating StatsTracker Values...");
        Kills = 0;
        Deaths = 0;
        timeHeld = 0;
        KDR = 0.0f;
        isDonutKing = false;
        moneyTotal = 150;//starting money for players and AI
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

    public void depositMoney(int money) { moneyTotal += money; }

    public void updateTimeHeld() { timeHeld++; }

    public void updateRoundsWon() { ++RoundsWon; }

    public int getRoundsWon() { return RoundsWon; }

    public int getTimeHeld() { return timeHeld; }

    public void resetTimeHeld() { timeHeld = 0; }

    //public void updateScore(int score) { scorePoints += score; }

    public void withdrawMoney(int money) { moneyTotal -= money; }

    public int getMoneyTotal() { return moneyTotal; }

    //Debug Method
    public string getAllStats()
    {
        string debugStats;
        debugStats = "\tK: " + Kills.ToString() + " | ";
        debugStats += "D: " + Deaths.ToString() + " | ";
        debugStats += "TH: " + timeHeld.ToString() + " | ";
        debugStats += "KDR: " + KDR.ToString("N2") + "% | ";
        debugStats += "DK?: " + isDonutKing.ToString() + " | ";
        debugStats += "$" + moneyTotal.ToString() + " | ";
        debugStats += "RW: " + RoundsWon.ToString() + " | ";
        return debugStats;
    }

    //METHOD FOR PRINTING STATS FOR SCOREBOARD
    public string GetScoreStats()
    {
        string scoreStats;
        scoreStats = " " + timeHeld.ToString() + "|";
        scoreStats += " R: " + RoundsWon.ToString() + "|";
        return scoreStats;
    }
}
