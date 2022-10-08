using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class DiscordHandshaker : MonoBehaviour
{
    // Cached References
    ScoreKeep scoreKeep = null;

    // Methods
    private void Awake()
    {
        scoreKeep = ScoreKeep.GetScoreKeep();
    }

    private void Start()
    {
        scoreKeep.ResetGame("dude", 20);
    }

    public void Setup(string jsonInput)
    {
        scoreKeep.ResetGame();
    }
}
