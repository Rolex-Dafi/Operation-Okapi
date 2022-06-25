using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterScriptableObject")]
public class CharacterScriptableObject : ScriptableObject
{
    public int money;
    public int health;
    public int respect; // only applicable to player character

}
