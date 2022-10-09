using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class DiscordHandshaker : MonoBehaviour
{
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
        scoreKeep.matchComplete += HandleMatchComplete;
    }

    private void OnDisable()
    {
        scoreKeep.rollComplete -= HandleRollComplete;
        scoreKeep.matchComplete -= HandleMatchComplete;
    }

    private void Start()
    {
        // Debug
        Setup(GenerateTestInitiator());
    }

    // Public Methods
    public void Setup(string jsonInput)
    {
        string decryptedMatchString = SymmetricEncryptor.DecryptToString(jsonInput);
        MatchData matchData = JsonUtility.FromJson<MatchData>(decryptedMatchString);
        if (matchData == null) { return; }
        scoreKeep.ResetMatch(matchData);
    }

    // Private Methods
    private void HandleRollComplete(bool isPlayer)
    {
        MatchData matchData = scoreKeep.GetMatchData();
        string matchString = JsonUtility.ToJson(matchData);
        UnityEngine.Debug.Log(SymmetricEncryptor.EncryptString(matchString));
    }

    private void HandleMatchComplete(MatchData matchData)
    {
        string matchString = JsonUtility.ToJson(matchData);
        UnityEngine.Debug.Log(SymmetricEncryptor.EncryptString(matchString));
    }

    private string GenerateTestInitiator()
    {
        MatchData matchData = new MatchData("CoolDude", 222);
        string matchString = JsonUtility.ToJson(matchData);
        string encryptedMatchString = SymmetricEncryptor.EncryptString(matchString);
        return encryptedMatchString;
    }
}
