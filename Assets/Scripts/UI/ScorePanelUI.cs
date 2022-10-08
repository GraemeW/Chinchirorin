using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScorePanelUI : MonoBehaviour
{
    [Header("Hookups")]
    [SerializeField] TMP_Text scoreNameField = null;
    [SerializeField] TMP_Text scoreField = null;

    public void Setup(string scoreName, int score)
    {
        string parsedScore = "-"; 
        if (score >= 0) { parsedScore = score.ToString(); }

        Setup(scoreName, parsedScore);
    }

    public void Setup(string scoreName, string score)
    {
        scoreNameField.text = scoreName;
        scoreField.text = score;
    }
}
