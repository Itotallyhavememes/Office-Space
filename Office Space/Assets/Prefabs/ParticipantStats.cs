using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticipantStats
{
    string DisplayName;
    int Kills, Deaths, timeHeld;
    float KDR;
    bool isDonutKing;
    int moneyTotal;

    public ParticipantStats instantiateStats()
    {
        Debug.Log("Instantiating StatsTracker Values...");
        Kills = 0;
        Deaths = 0;
        timeHeld = 0;
        KDR = 0.0f;
        isDonutKing = false;
        moneyTotal = 500;//starting money for players and AI
        return this;
    }

    //Methods to adjust struct values
    public void setDisplayName(string displayName) { DisplayName = displayName; }

    public void updateKills() { ++Kills; }

    public void updateDeaths() {  ++Deaths; }

    public int getDeaths() { return Deaths; }

    public int getKills() { return Kills; }

    public void getKDR() { KDR = Kills / Deaths; }

    public void updateDKStatus() { isDonutKing = !isDonutKing; }

    public bool getDKStatus()
    {
        if (isDonutKing)
            return true;
        else
            return false;
    }

    public void depositMoney(int money) { moneyTotal += money; }

    public void withdrawMoney(int money) { moneyTotal -= money; }

    public int getMoneyTotal() { return moneyTotal; }

    //Debug Method
    public string getAllStats()
    {
        string debugStats;
        debugStats = "K: " + Kills.ToString() + " | ";
        debugStats += "D: " + Deaths.ToString() + " | ";
        debugStats += "TH: " + timeHeld.ToString() + " | ";
        debugStats += "KDR: " + KDR.ToString() + " | ";
        debugStats += "DK?: " + isDonutKing.ToString() + " | ";
        debugStats += "$" + moneyTotal.ToString() + " | ";
        return debugStats;
    }
}
