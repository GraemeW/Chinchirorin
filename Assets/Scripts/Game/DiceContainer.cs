using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceContainer : MonoBehaviour
{
    // Tunables
    [SerializeField] Dice dicePrefab = null;

    // State
    bool isPlayer = true;
    int maxDiceToSettle = 0;
    int settledDiceCount = 0;
    int[] settledDice;

    // Events
    public event Action<int[]> diceScoreSettled;

    // Public Methods
    public void SetPlayer(bool isPlayer) { this.isPlayer = isPlayer; }
    public bool IsPlayer() => isPlayer;

    public void SpawnDice(Transform target, int quantity = 3, bool isPlayer = true)
    {
        maxDiceToSettle = quantity;
        settledDice = new int[quantity];

        for (int i = 0; i < quantity; i++)
        {
            Dice dice = Instantiate(dicePrefab, transform);
            dice.Setup(target.position, UnityEngine.Random.rotation, i);
            dice.diceRollComplete += SettleDice;
        }
    }

    public void ResetDice()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        maxDiceToSettle = 0;
        settledDiceCount = 0;
    }

    // Private Methods
    private void SettleDice(Dice dice)
    {
        settledDice[dice.GetIndex()] = dice.GetUpValue();
        dice.diceRollComplete -= SettleDice;
        settledDiceCount++;

        if (settledDiceCount == maxDiceToSettle)
        {
            diceScoreSettled?.Invoke(settledDice);
        }
    }
}
