using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player character data class.
/// </summary>
[CreateAssetMenu(fileName = "PlayerCharacterData", menuName = "ScriptableObjects/Player Character")]
public class PlayerCharacterSO : CharacterSO
{
    public int respect; 

    public List<ItemSO> startingItems;
}
