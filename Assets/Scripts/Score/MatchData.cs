using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatchData
{
    public string matchResolutionType;
    public int betWinnings;
    public string playerName;
    public int playerScore;
    public string opponentName;
    public int opponentScore;
    public int throwCountTracker;
    public long timeStamp;

    public MatchData(MatchResolutionType matchResolutionType, int winnings, string playerName, int playerScore, string opponentName, int opponentScore, int throwCountTracker)
    {
        this.matchResolutionType = Enum.GetName(typeof(MatchResolutionType), matchResolutionType);
        betWinnings = winnings;
        this.playerName = playerName;
        this.playerScore = playerScore;
        this.opponentName = opponentName;
        this.opponentScore = opponentScore;
        this.throwCountTracker = throwCountTracker;
        this.timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    public MatchData(string playerName, int bet)
    {
        matchResolutionType = Enum.GetName(typeof(MatchResolutionType), MatchResolutionType.InProgress); // New game starts as InProgress
        betWinnings = bet;
        this.playerName = playerName;
        playerScore = 0;
        opponentName = "";
        opponentScore = 0;
        throwCountTracker = 0;
        this.timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();
    }
}
