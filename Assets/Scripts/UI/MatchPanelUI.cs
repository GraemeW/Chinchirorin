using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchPanelUI : MonoBehaviour
{
    // Tunables
    [Header("Properties")]
    [SerializeField] string defaultDrawName = "Nobody";
    [SerializeField] string defaultWonTitle = "Won";
    [SerializeField] string defaultLostTitle = "Lost";
    [Header("Hookups")]
    [SerializeField] TMP_Text winnerField = null;
    [SerializeField] TMP_Text winningsDispositionField = null;
    [SerializeField] TMP_Text winningsField = null;
    [SerializeField] TMP_Text playerNameField = null;
    [SerializeField] TMP_Text playerScoreField = null;
    [SerializeField] TMP_Text opponentNameField = null;
    [SerializeField] TMP_Text opponentScoreField = null;
    [SerializeField] GameObject specialScorePanel = null;
    [SerializeField] GameObject standardScorePanel = null;

    public void Setup(MatchData matchData, ScoreType scoreType)
    {
        if (Enum.TryParse(matchData.matchResolutionType, out MatchResolutionType matchResolutionType))
        {
            switch (matchResolutionType)
            {
                case MatchResolutionType.PlayerWin:
                    winnerField.text = matchData.playerName;
                    winningsDispositionField.text = defaultWonTitle;
                    break;
                case MatchResolutionType.PlayerLoss:
                    winnerField.text = matchData.opponentName;
                    winningsDispositionField.text = defaultLostTitle;
                    break;
                case MatchResolutionType.Draw:
                    winnerField.text = defaultDrawName;
                    break;
                case MatchResolutionType.InProgress:
                    break;
            }
        }

        winningsField.text = $"${Mathf.Abs(matchData.betWinnings)}";
        playerNameField.text = matchData.playerName;
        playerScoreField.text = matchData.playerScore.ToString();
        opponentNameField.text = matchData.opponentName;
        opponentScoreField.text = matchData.opponentScore.ToString();

        switch (scoreType)
        {
            case ScoreType.Whiff:
            case ScoreType.Standard:
                standardScorePanel.SetActive(true);
                specialScorePanel.SetActive(false);
                break;
            case ScoreType.StormDouble:
            case ScoreType.StormTriple:
                specialScorePanel.SetActive(true);
                standardScorePanel.SetActive(false);
                break;
        }
    }
}
