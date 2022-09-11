using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Merchant character data class.
/// </summary>
[CreateAssetMenu(fileName = "MerchantCharacterData", menuName = "ScriptableObjects/Merchant Character")]
public class MerchantCharacterSO : CharacterSO
{
    public List<ItemSO> shop;

}
