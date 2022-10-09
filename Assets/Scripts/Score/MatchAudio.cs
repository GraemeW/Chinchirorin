using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchAudio : MonoBehaviour
{
    [Header("Match Resolution")]
    [SerializeField] SoundEffects winSounds = null;
    [SerializeField] SoundEffects loseSounds = null;
    [SerializeField] SoundEffects drawSounds = null;

    // Cached References
    ScoreKeep scoreKeep = null;

    // Unity Methods
    private void Awake()
    {
        scoreKeep = ScoreKeep.GetScoreKeep();
    }

    private void OnEnable()
    {
        scoreKeep.matchComplete += HandleMatchComplete;
    }

    private void OnDisable()
    {
        scoreKeep.matchComplete -= HandleMatchComplete;
    }

    // Private Methods
    private void HandleMatchComplete(MatchData matchData)
    {
        if (Enum.TryParse(matchData.matchResolutionType, out MatchResolutionType matchResolutionType))
        {
            switch (matchResolutionType)
            {
                case MatchResolutionType.PlayerWin:
                    winSounds.PlayRandomAudioClip();
                    break;
                case MatchResolutionType.PlayerLoss:
                    loseSounds.PlayRandomAudioClip();
                    break;
                case MatchResolutionType.Draw:
                    drawSounds.PlayRandomAudioClip();
                    break;
                case MatchResolutionType.InProgress:
                    break;
            }
        }
    }
}
