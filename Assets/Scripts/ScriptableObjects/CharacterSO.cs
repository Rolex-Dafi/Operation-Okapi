using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/Character")]
public class CharacterSO : ScriptableObject
{
    public int money;
    public int health;
    public int respect; // only applicable to player character

    // sound
    public FMODUnity.EventReference onHitSound;
    public FMODUnity.EventReference onDeathSound;

}
