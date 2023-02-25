using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardLoader : MonoBehaviour
{
    // [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text statText;

    
    public void LoadCard(string nameText, string stats) {
        SetNameText(nameText);
        SetStatText(stats);
    }

    void SetNameText(string name) {
        nameText.text = name;
    }

    void SetStatText(string text) {
        statText.text = text;
    }
}
