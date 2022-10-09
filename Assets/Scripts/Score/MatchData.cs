using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatchData
{
    public MatchResolutionType matchResolutionType;
    public int betWinnings;
    public string playerName;
    public int playerScore;
    public string opponentName;
    public int opponentScore;
    public int throwCountTracker;

    public MatchData(MatchResolutionType matchResolutionType, int winnings, string playerName, int playerScore, string opponentName, int opponentScore, int throwCountTracker)
    {
        this.matchResolutionType = matchResolutionType;
        betWinnings = winnings;
        this.playerName = playerName;
        this.playerScore = playerScore;
        this.opponentName = opponentName;
        this.opponentScore = opponentScore;
        this.throwCountTracker = throwCountTracker;
    }

    public MatchData(string playerName, int bet)
    {
        matchResolutionType = MatchResolutionType.InProgress;
        betWinnings = bet;
        this.playerName = playerName;
        playerScore = 0;
        opponentName = "";
        opponentScore = 0;
        throwCountTracker = 0;
    }
}
