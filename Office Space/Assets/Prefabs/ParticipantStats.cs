using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct ParticipantStats
{
    string DisplayName;
    int Kills, Deaths, timeHeld;
    float KDR;
    bool isDonutKing;
    int scorePoints;

    public ParticipantStats instantiateStats()
    {
        Kills = 0;
        Deaths = 0;
        timeHeld = 0;
        KDR = 0.0f;
        isDonutKing = false;
        scorePoints = 0;
        return this;
    }

    //Methods to adjust struct values
    public void setDisplayName(string displayName) { DisplayName = displayName; }

    public void updateKills(int kills) { Kills += kills; }

    public void updateDeaths(int deaths) { Deaths += deaths; }

    public void getKDR() { KDR = Kills / Deaths; }

    public void updateDKStatus() { isDonutKing = !isDonutKing; }

    public bool getDKStatus()
    {
        if (isDonutKing)
            return true;
        else
            return false;
    }

    public void updateScore(int score) { scorePoints += score; }

    //Debug Method
    public string getAllStats()
    {
        string debugStats;
        debugStats = "K: " + Kills.ToString() + " | ";
        debugStats += "D: " + Deaths.ToString() + " | ";
        debugStats += "TH: " + timeHeld.ToString() + " | ";
        debugStats += "KDR: " + KDR.ToString() + " | ";
        debugStats += "DK?: " + isDonutKing.ToString() + " | ";
        debugStats += "S: " + scorePoints.ToString() + " | ";
        return debugStats;
    }
}
