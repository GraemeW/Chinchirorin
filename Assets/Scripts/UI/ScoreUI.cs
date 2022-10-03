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
    [SerializeField] Canvas uiCanvas = null;
    [SerializeField] TMP_Text rollMessage = null;
    [SerializeField] Transform diceEntryPanel = null;
    [SerializeField] DiceEntryUI diceEntryPrefab = null;

    // Cached References
    ScoreKeep scoreKeep = null;

    // Unity Methods
    private void Awake()
    {
        scoreKeep = ScoreKeep.GetScoreKeep();
    }

    private void OnEnable()
    {
        scoreKeep.rollComplete += UpdateUI;
    }

    private void OnDisable()
    {
        scoreKeep.rollComplete -= UpdateUI;
    }

    private void Start()
    {
        uiCanvas.gameObject.SetActive(false);
    }

    // Public Methods
    public void ResetGame() // Called via Unity Events
    {
        foreach(Transform child in diceEntryPanel)
        {
            Destroy(child.gameObject);
        }
        uiCanvas.gameObject.SetActive(false);
        scoreKeep.ResetGame();
    }

    // Private Methods
    private void UpdateUI()
    {
        if (uiCanvas == null) { return; }
        uiCanvas.gameObject.SetActive(true);

        if (diceEntryPanel == null) { return; }

        ScoreType scoreType = scoreKeep.GetScoreType();
        int score = scoreKeep.GetScore();
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
                    rollMessage.text = messageStormDoublePositive;
                }
                else
                {
                    rollMessage.text = messageStormDoubleNegative;
                }
                break;
            case ScoreType.StormTriple:
                if (score > 0)
                {
                    rollMessage.text = messageStormTriplePositive;
                }
                else
                {
                    rollMessage.text = messageStormTripleNegative;
                }
                break;
        }

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
