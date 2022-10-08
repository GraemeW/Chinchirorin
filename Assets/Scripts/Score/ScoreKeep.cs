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
    [SerializeField] int defaultWager = 10;
    [Header("Hookups")]
    [SerializeField] DiceContainer diceContainer = null;
    [SerializeField] ThrowHand throwHand = null;

    // State
    string playerName = null;
    string opponentName = "";
    int wager = 0;

    int[] currentRolls;
    ScoreType currentScoreType = ScoreType.Whiff;
    int playerScore = 0;
    int opponentScore = 0;

    bool isGameComplete = false;

    // Events
    public event Action<bool> rollComplete;
    public event Action throwReset;
    public event Action gameComplete;
    public event Action gameReset;

    // Unity Methods
    private void Awake()
    {
        wager = defaultWager;
    }

    private void OnEnable()
    {
        if (diceContainer == null) { return; }
        diceContainer.diceScoreSettled += HandleRollComplete;
    }

    private void OnDisable()
    {
        if (diceContainer == null) { return; }
        diceContainer.diceScoreSettled -= HandleRollComplete;
    }

    // Public Methods
    public string GetPlayerName() => playerName;
    public string GetOpponentName() => opponentName;
    public int GetWager() => wager;
    public ScoreType GetScoreType() => currentScoreType;
    public int GetScore(bool isPlayer) => isPlayer ? playerScore : opponentScore;
    public int[] GetRolls() => currentRolls;

    public void ResetThrow()
    {
        if (diceContainer == null) { return; }
        diceContainer.ResetDice();
        if (throwHand == null) { return; }
        throwHand.ResetThrow();

        currentRolls = new int[0];
        currentScoreType = ScoreType.Whiff;

        throwReset?.Invoke();
    }
    public void ResetGame(string playerName = null, int wager = 0)
    {
        isGameComplete = false;

        this.playerName = playerName;
        this.wager = wager;
        opponentName = NameGenerator.GetRandomName();
        playerScore = 0;
        opponentScore = 0;
        
        throwHand.ResetGame();
        ResetThrow();

        gameReset?.Invoke();
    }

    // Private Methods
    private void HandleRollComplete(int[] rolls)
    {
        currentRolls = (int[])rolls.Clone();
        Array.Sort(currentRolls);

        bool? isPlayer = diceContainer.IsPlayer();
        if (isPlayer.HasValue)
        {
            ProcessStandardScore(isPlayer.Value);
            rollComplete?.Invoke(isPlayer.Value);
            if (isGameComplete) { gameComplete?.Invoke(); }
        }
    }

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
                SetScore(isPlayer, 1);
            }
            else
            {
                currentScoreType = ScoreType.StormTriple;
                SetScore(isPlayer, -1);
            }
            isGameComplete = true;
        }
        else if (firstEntry == 4 && secondEntry == 5 && thirdEntry == 6)
        {
            currentScoreType = ScoreType.StormDouble;
            SetScore(isPlayer, 1);
            isGameComplete = true;
        }
        else if (firstEntry == 1 && secondEntry == 2 && thirdEntry == 3)
        {
            currentScoreType = ScoreType.StormDouble;
            SetScore(isPlayer, -1);
            isGameComplete = true;
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

        if (!throwHand.AnyThrowsRemaining()) { isGameComplete = true; }
    }

    private void SetScore(bool isPlayer, int score)
    {
        if (isPlayer)
        {
            playerScore = Mathf.Max(playerScore, score);
        }
        else
        {
            opponentScore = Mathf.Max(opponentScore, score);
        }
    }
}
