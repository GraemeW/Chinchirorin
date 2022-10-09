using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class DiscordHandshaker : MonoBehaviour
{
    // Tunables
    [SerializeField] bool debugGenerateTestInitiator = false;

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
        if (debugGenerateTestInitiator) { Setup(GenerateTestInitiator()); }

        // ---- Other Debug
        // ---- Working Inputs:
        // -- Fresh game

        // -- Fourth roll complete
        //Setup("Jym/DRAwKgKTQg54aL/j1pbT317/RVp/Yz18wWahsPdHZHxcDLj3GuBGJKN1rvGRpOHQu/txLyODEAfGbO6m+YC42AXF5e/aOAcJjITWN7Ne0WAFj1kkoSld2IvPZ7thAJUfhwzD3Fg1/3aD0GeIDb0pcObCebzD0zPDuycjNlcgucOXuXfspg0KdSIu9ON2SZBuvKujUgOSE/vT80VftQ==");

        // ----Garbage Inputs:
        //Setup("garbage");
        //Setup("Jym/DRAwKgKTQg54aL/j1pbT317/RVp/akjsdhkjasdkjhiu/txLyODEAfGbO6m+YC42AXF5e/ijeiuiuewhiuweiuhre/3aD0GeIDb0pcObCebzD0zPDuycjNlcgucOXuXfspg0KdSIu9ON2SZBuvKujUgOSE/vT80VftQ==");
    }

    // Public Methods
    public void Setup(string jsonInput)
    {
        UnityEngine.Debug.Log($"Attempting to decrypt: {jsonInput}");
        string decryptedMatchString = "";
        try
        {
            decryptedMatchString = SymmetricEncryptor.DecryptToString(jsonInput);
        }
        catch { UnityEngine.Debug.Log("Failed to decrypt json string"); return; }

        UnityEngine.Debug.Log("Success!  Loading game...");
        MatchData matchData = JsonUtility.FromJson<MatchData>(decryptedMatchString);
        if (matchData == null) { UnityEngine.Debug.Log("Failed to convert decrypted string to match data"); return; }
        scoreKeep.ResetMatch(matchData);

        UnityEngine.Debug.Log($"Successfully loaded game: {decryptedMatchString}");
    }

    // Private Methods
    private void HandleRollComplete(bool isPlayer)
    {
        MatchData matchData = scoreKeep.GetMatchData();
        string matchString = JsonUtility.ToJson(matchData);
        UnityEngine.Debug.Log("Roll complete, posting:");
        UnityEngine.Debug.Log(SymmetricEncryptor.EncryptString(matchString));
    }

    private void HandleMatchComplete(MatchData matchData)
    {
        string matchString = JsonUtility.ToJson(matchData);
        UnityEngine.Debug.Log("Match complete, posting:");
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
