using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MerchantCharacterData", menuName = "ScriptableObjects/Merchant Character")]
public class MerchantCharacterSO : CharacterSO
{
    public List<ItemSO> shop;

}
