using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreKeep : MonoBehaviour
{
    // Static finder for UI
    public static ScoreKeep GetScoreKeep()
    {
        GameObject scoreKeepGameObject = GameObject.FindGameObjectWithTag("ScoreKeep");
        if (scoreKeepGameObject.TryGetComponent(out ScoreKeep scoreKeep))
        {
            return scoreKeep;
        }
        return null; 
    }

    // Tunables
    [Header("Properties")]
    [SerializeField] string defaultPlayerName = "Player";
    [SerializeField] int stormWinScore = 999;
    [SerializeField] int defaultBet = 10;
    [SerializeField] int throwCount = 3;
    [Header("Hookups")]
    [SerializeField] DiceContainer diceContainer = null;
    [SerializeField] ThrowHand throwHand = null;

    // State
    string playerName = null;
    string opponentName = "";
    int bet = 0;

    int[] currentRolls;
    ScoreType currentScoreType = ScoreType.Whiff;
    int playerScore = 0;
    int opponentScore = 0;

    int throwCountTracker = 0;
    bool isMatchComplete = false;

    // Events
    public event Action<bool> rollComplete;
    public event Action throwReset;
    public event Action<MatchData> matchComplete;
    public event Action matchReset;

    // Unity Methods
    private void Awake()
    {
        bet = defaultBet;
    }

    private void OnEnable()
    {
        if (throwHand == null) { return; }
        throwHand.throwComplete += HandleThrowComplete;
        if (diceContainer == null) { return; }
        diceContainer.diceScoreSettled += HandleRollComplete;
    }

    private void OnDisable()
    {
        if (throwHand == null) { return; }
        throwHand.throwComplete -= HandleThrowComplete;
        if (diceContainer == null) { return; }
        diceContainer.diceScoreSettled -= HandleRollComplete;
    }

    // Public Methods
    public string GetPlayerName()
    {
        if (string.IsNullOrWhiteSpace(playerName)) { return defaultPlayerName; }
        return playerName;
    }
    public string GetOpponentName() => opponentName;
    public ScoreType GetScoreType() => currentScoreType;
    public int GetScore(bool isPlayer) => isPlayer ? playerScore : opponentScore;
    public int[] GetRolls() => currentRolls;
    public bool ShouldAIThrow()
    {
        return throwCountTracker >= throwCount && throwCountTracker < throwCount * 2;
    }
    public MatchData GetMatchData()
    {
        int betWinnings = isMatchComplete ? GetWinnings() : bet;
        MatchData matchResolutionData = new MatchData(GetMatchResolutionType(), betWinnings, playerName, playerScore, opponentName, opponentScore, throwCountTracker);
        return matchResolutionData;
    }

    public void ResetThrow()
    {
        // Dice & hand state reset
        if (diceContainer == null) { return; }
        diceContainer.ResetDice();
        if (throwHand == null) { return; }
        throwHand.SetAIThrowing(ShouldAIThrow());
        throwHand.ResetThrow();

        // Score state reset
        currentRolls = new int[0];
        currentScoreType = ScoreType.Whiff;

        throwReset?.Invoke();
    }

    public void ResetMatch(MatchData matchData)
    {
        isMatchComplete = false;

        bet = matchData.betWinnings;
        playerName = matchData.playerName;
        opponentName = string.IsNullOrWhiteSpace(matchData.opponentName) ? NameGenerator.GetRandomName() : matchData.opponentName;
        opponentScore = matchData.opponentScore;
        playerScore = matchData.playerScore;
        throwCountTracker = matchData.throwCountTracker;
        
        ResetThrow();

        matchReset?.Invoke();
    }

    public void ResetMatch()
    {
        // For manual call via Unity Events (not called in normal flow)
        MatchData matchData = new MatchData(GetPlayerName(), bet);
        ResetMatch(matchData);
    }

    // Private Methods
    private void HandleThrowComplete()
    {
        throwCountTracker++;
    }

    private void HandleRollComplete(int[] rolls)
    {
        // Process score -- clone since sort done in-place
        currentRolls = (int[])rolls.Clone();
        Array.Sort(currentRolls);
        ProcessStandardScore(diceContainer.IsPlayer());

        // && Reset to next roll (or otherwise call match
        if (isMatchComplete)
        {
            MatchData matchResolutionData = GetMatchData();
            matchComplete?.Invoke(matchResolutionData);
        }
        else
        {
            rollComplete?.Invoke(diceContainer.IsPlayer());
        }
    }

    private bool AnyThrowsRemaining() => throwCountTracker < throwCount * 2; // *2 for 1st: player throws, 2nd: opponent throws

    private void ProcessStandardScore(bool isPlayer)
    {
        if (currentRolls.Length != 3) { return; }

        // Handling 3 x 6-sided dice for standard flow -- could generalize rules, but out of scope
        int firstEntry = currentRolls[0];
        int secondEntry = currentRolls[1];
        int thirdEntry = currentRolls[2];

        if (firstEntry == secondEntry && secondEntry == thirdEntry)
        {
            if (firstEntry >= 4)
            {
                currentScoreType = ScoreType.StormTriple;
                SetScore(isPlayer, stormWinScore, true);
            }
            else
            {
                currentScoreType = ScoreType.StormTriple;
                SetScore(isPlayer, -stormWinScore, true);
            }
            isMatchComplete = true;
        }
        else if (firstEntry == 4 && secondEntry == 5 && thirdEntry == 6)
        {
            currentScoreType = ScoreType.StormDouble;
            SetScore(isPlayer, stormWinScore, true);
            isMatchComplete = true;
        }
        else if (firstEntry == 1 && secondEntry == 2 && thirdEntry == 3)
        {
            currentScoreType = ScoreType.StormDouble;
            SetScore(isPlayer, -stormWinScore, true);
            isMatchComplete = true;
        }
        else if (firstEntry == secondEntry)
        {
            currentScoreType = ScoreType.Standard;
            SetScore(isPlayer, thirdEntry);
        }
        else if (firstEntry == thirdEntry)
        {
            currentScoreType = ScoreType.Standard;
            SetScore(isPlayer, secondEntry);
        }
        else if (secondEntry == thirdEntry)
        {
            currentScoreType = ScoreType.Standard;
            SetScore(isPlayer, firstEntry);
        }
        else
        {
            currentScoreType = ScoreType.Whiff;
            SetScore(isPlayer, 0);
        }

        if (!AnyThrowsRemaining()) { isMatchComplete = true; }
    }

    private void SetScore(bool isPlayer, int score, bool forceScore = false)
    {
        if (isPlayer)
        {
            if (forceScore) { playerScore = score; }
            else { playerScore = Mathf.Max(playerScore, score); }
        }
        else
        {
            if (forceScore) { opponentScore = score; }
            else { opponentScore = Mathf.Max(opponentScore, score); }
        }
    }

    private MatchResolutionType GetMatchResolutionType()
    {
        if (!isMatchComplete) { return MatchResolutionType.InProgress; }

        if (playerScore == opponentScore) { return MatchResolutionType.Draw; }
        else if (playerScore > opponentScore) { return MatchResolutionType.PlayerWin; }
        else { return MatchResolutionType.PlayerLoss; }
    }

    private int GetWinnings()
    {
        int sign = 0;
        switch (GetMatchResolutionType())
        {
            case MatchResolutionType.PlayerWin:
                sign = 1;
                break;
            case MatchResolutionType.PlayerLoss:
                sign = -1;
                break;
            case MatchResolutionType.Draw:
            case MatchResolutionType.InProgress:
                break;
        }
        if (sign == 0) { return 0; }

        int multiplier = 1;
        switch (currentScoreType)
        {
            case ScoreType.StormDouble:
                multiplier = 2;
                break;
            case ScoreType.StormTriple:
                multiplier = 3;
                break;
            case ScoreType.Whiff:
            case ScoreType.Standard:
                break;
        }

        return bet * sign * multiplier;
    }
}
