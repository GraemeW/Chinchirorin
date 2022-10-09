using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("Tunables")]
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
    [SerializeField] TMP_Text rollMessage = null;
    [SerializeField] Transform diceEntryPanel = null;
    [SerializeField] DiceEntryUI diceEntryPrefab = null;
    [SerializeField] MatchPanelUI matchCompletePanel = null;

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
        scoreKeep.matchComplete += HandleMatchComplete;
        scoreKeep.matchReset += HandleMatchReset;
    }

    private void OnDisable()
    {
        scoreKeep.rollComplete -= HandleRollComplete;
        scoreKeep.throwReset -= HandleThrowReset;
        scoreKeep.matchComplete -= HandleMatchComplete;
        scoreKeep.matchReset -= HandleMatchReset;
    }

    private void Start()
    {
        rollPanel.SetActive(false);
        matchCompletePanel.gameObject.SetActive(false);
    }

    // Public Methods
    public void TriggerThrowReset() // Called via Unity Events --> calls back to HandleReset
    {
        if (scoreKeep == null) { return; }
        scoreKeep.ResetThrow();
    }

    public void TriggerMatchReset()  // Called via Unity Events --> calls back to HandleReset
    {
        if (scoreKeep == null) { return; }
        scoreKeep.ResetMatch();
    }

    // Private Methods
    private void HandleRollComplete(bool isPlayer)
    {
        rollPanel.gameObject.SetActive(true);

        ScoreType scoreType = scoreKeep.GetScoreType();
        int score = scoreKeep.GetScore(isPlayer);
        UpdateRollMessage(isPlayer, scoreType, score);
        ShowDiceRolls(scoreType, score);
        UpdateScores(isPlayer, score);
    }

    private void HandleMatchComplete(MatchData matchResolutionData)
    {
        if (scoreKeep == null) { return; }

        // Safety against roll panel showing on match end -- suppress for game complete
        rollPanel.gameObject.SetActive(false);

        // Then set up the main match panel
        matchCompletePanel.gameObject.SetActive(true);
        matchCompletePanel.Setup(matchResolutionData, scoreKeep.GetScoreType());
    }

    private void HandleThrowReset()
    {
        foreach (Transform child in diceEntryPanel)
        {
            Destroy(child.gameObject);
        }
        rollPanel.gameObject.SetActive(false);
        if (scoreKeep.ShouldAIThrow())
        {

        }
    }

    private void HandleMatchReset()
    {
        if (scoreKeep == null) { return; }

        playerScorePanel.Setup(scoreKeep.GetPlayerName(), "-");
        opponentScorePanel.Setup(scoreKeep.GetOpponentName(), "-");

        matchCompletePanel.gameObject.SetActive(false);
    }

    private void UpdateScores(bool isPlayer, int score)
    {
        if (scoreKeep == null) { return; }

        if (isPlayer)
        {
            playerScorePanel.Setup(scoreKeep.GetPlayerName(), score);
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
        if (scoreKeep == null) { return; }

        int[] rolls = scoreKeep.GetRolls();
        foreach (int roll in rolls)
        {
            DiceEntryUI diceEntry = Instantiate(diceEntryPrefab, diceEntryPanel);
            diceEntry.Setup(roll);
        }
    }
}
