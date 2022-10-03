using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiceEntryUI : MonoBehaviour
{
    [SerializeField] TMP_Text entryText = null;

    public void Setup(int value)
    {
        entryText.text = value.ToString();
    }
}
