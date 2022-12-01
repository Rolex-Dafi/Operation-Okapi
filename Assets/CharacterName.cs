using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    public void Init(string charName)
    {
        nameText.text = charName;
    }
}
