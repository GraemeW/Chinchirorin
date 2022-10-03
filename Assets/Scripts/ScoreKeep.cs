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
    [SerializeField] int initialCash = 1000;
    [Header("Hookups")]
    [SerializeField] DiceContainer diceContainer = null;
    [SerializeField] ThrowHand throwHand = null;

    // State
    int[] currentRolls;
    ScoreType currentScoreType = ScoreType.Whiff;
    int currentScore = 0;
    int currentCash = 0;

    // Events
    public event Action rollComplete;

    // Unity Methods
    private void Awake()
    {
        currentCash = initialCash;
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
    public ScoreType GetScoreType() => currentScoreType;
    public int GetScore() => currentScore;

    public int[] GetRolls() => currentRolls;
    public int GetCash() => currentCash;
    public void ResetGame()
    {
        if (diceContainer == null) { return; }
        diceContainer.ResetDice();
        if (throwHand == null) { return; }
        throwHand.ResetThrow();

        currentRolls = new int[0];
        currentScoreType = ScoreType.Whiff;
        currentScore = 0;
    }

    // Private Methods
    private void HandleRollComplete(int[] rolls)
    {
        currentRolls = (int[])rolls.Clone();
        Array.Sort(currentRolls);

        ProcessStandardScore();
        rollComplete?.Invoke();
    }

    private void ProcessStandardScore()
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
                currentScore = 1;
            }
            else
            {
                currentScoreType = ScoreType.StormTriple;
                currentScore = -1;
            }
        }
        else if (firstEntry == 4 && secondEntry == 5 && thirdEntry == 6)
        {
            currentScoreType = ScoreType.StormDouble;
            currentScore = 1;
        }
        else if (firstEntry == 1 && secondEntry == 2 && thirdEntry == 3)
        {
            currentScoreType = ScoreType.StormDouble;
            currentScore = -1;
        }
        else if (firstEntry == secondEntry)
        {
            currentScoreType = ScoreType.Standard;
            currentScore = thirdEntry;
        }
        else if (firstEntry == thirdEntry)
        {
            currentScoreType = ScoreType.Standard;
            currentScore = secondEntry;
        }
        else if (secondEntry == thirdEntry)
        {
            currentScoreType = ScoreType.Standard;
            currentScore = firstEntry;
        }
        else
        {
            currentScoreType = ScoreType.Whiff;
            currentScore = 0;
        }
    }
}
