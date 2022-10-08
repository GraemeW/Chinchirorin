using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("Tunables")]
    [SerializeField] string defaultPlayerName = "Player";
    [SerializeField] string messageWhiff = "Whiff!";
    [SerializeField] string messageStandard = "Score:";
    [SerializeField] string messageStormDoublePositive = "Storm!  Double!!";
    [SerializeField] string messageStormTriplePositive = "Storm!  Triple!!!";
    [SerializeField] string messageStormDoubleNegative = "Storm!  Oops!!";
    [SerializeField] string messageStormTripleNegative = "Storm!  Yikes!!!";

    [Header("Hookups")]
    [SerializeField] GameObject rollPanel = null;
    [SerializeField] ScorePanelUI playerScorePanel = null;
    [SerializeField] ScorePanelUI opponentScorePanel = null;
    [SerializeField] GameObject rollAgainPanel = null;
    [SerializeField] GameObject playAgainPanel = null;
    [SerializeField] TMP_Text rollMessage = null;
    [SerializeField] Transform diceEntryPanel = null;
    [SerializeField] DiceEntryUI diceEntryPrefab = null;

    // State

    // Cached References
    ScoreKeep scoreKeep = null;

    // Unity Methods
    private void Awake()
    {
        scoreKeep = ScoreKeep.GetScoreKeep();
    }

    private void OnEnable()
    {
        scoreKeep.rollComplete += HandleRollComplete;
        scoreKeep.throwReset += HandleThrowReset;
        scoreKeep.gameComplete += HandleGameComplete;
        scoreKeep.gameReset += HandleGameReset;
    }

    private void OnDisable()
    {
        scoreKeep.rollComplete -= HandleRollComplete;
        scoreKeep.throwReset -= HandleThrowReset;
        scoreKeep.gameComplete -= HandleGameComplete;
        scoreKeep.gameReset -= HandleGameReset;
    }

    private void Start()
    {
        rollPanel.gameObject.SetActive(false);
    }

    // Public Methods
    public void TriggerThrowReset() // Called via Unity Events --> calls back to HandleReset
    {
        if (scoreKeep == null) { return; }
        scoreKeep.ResetThrow();
    }

    public void TriggerGameReset()  // Called via Unity Events --> calls back to HandleReset
    {
        if (scoreKeep == null) { return; }
        scoreKeep.ResetGame();
    }

    // Private Methods
    private void HandleRollComplete(bool isPlayer)
    {
        if (rollPanel == null) { return; }
        rollPanel.gameObject.SetActive(true);

        playAgainPanel.SetActive(false);
        rollAgainPanel.SetActive(true);

        UpdateUI(isPlayer);
    }

    private void HandleGameComplete()
    {
        rollAgainPanel.SetActive(false);
        playAgainPanel.SetActive(true);
    }

    private void HandleThrowReset()
    {
        foreach (Transform child in diceEntryPanel)
        {
            Destroy(child.gameObject);
        }
        rollPanel.gameObject.SetActive(false);
    }

    private void HandleGameReset()
    {
        string playerName = scoreKeep.GetPlayerName() != null ? scoreKeep.GetPlayerName() : defaultPlayerName;
        playerScorePanel.Setup(playerName, "-");
        opponentScorePanel.Setup(scoreKeep.GetOpponentName(), "-");
    }

    private void UpdateUI(bool isPlayer)
    {
        if (diceEntryPanel == null) { return; }

        ScoreType scoreType = scoreKeep.GetScoreType();
        int score = scoreKeep.GetScore(isPlayer);
        UpdateRollMessage(isPlayer, scoreType, score);
        ShowDiceRolls(scoreType, score);
        UpdateScores(isPlayer, score);
    }

    private void UpdateScores(bool isPlayer, int score)
    {
        if (isPlayer)
        {
            string playerName = scoreKeep.GetPlayerName() != null ? scoreKeep.GetPlayerName() : defaultPlayerName;
            playerScorePanel.Setup(playerName, score);
        }
        else
        {
            opponentScorePanel.Setup(scoreKeep.GetOpponentName(), score);
        }
    }

    private void ShowDiceRolls(ScoreType scoreType, int score)
    {
        if (scoreType == ScoreType.Whiff || scoreType == ScoreType.Standard)
        {
            DiceEntryUI diceEntry = Instantiate(diceEntryPrefab, diceEntryPanel);
            diceEntry.Setup(score);
        }
        else
        {
            ShowAllDiceRolls();
        }
    }

    private void UpdateRollMessage(bool isPlayer, ScoreType scoreType, int score)
    {
        switch (scoreType)
        {
            case ScoreType.Whiff:
                rollMessage.text = messageWhiff;
                break;
            case ScoreType.Standard:
                rollMessage.text = messageStandard;
                break;
            case ScoreType.StormDouble:
                if (score > 0)
                {
                    rollMessage.text = isPlayer ? messageStormDoublePositive : messageStormDoubleNegative;
                }
                else
                {
                    rollMessage.text = isPlayer ? messageStormDoubleNegative : messageStormDoublePositive;
                }
                break;
            case ScoreType.StormTriple:
                if (score > 0)
                {
                    rollMessage.text = isPlayer ? messageStormTriplePositive : messageStormTripleNegative;
                }
                else
                {
                    rollMessage.text = isPlayer ? messageStormTripleNegative : messageStormTriplePositive;
                }
                break;
        }
    }

    private void ShowAllDiceRolls()
    {
        int[] rolls = scoreKeep.GetRolls();
        foreach (int roll in rolls)
        {
            DiceEntryUI diceEntry = Instantiate(diceEntryPrefab, diceEntryPanel);
            diceEntry.Setup(roll);
        }
    }
}
