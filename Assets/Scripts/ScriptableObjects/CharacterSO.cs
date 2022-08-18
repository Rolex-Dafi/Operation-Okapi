using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/Character")]
public class CharacterSO : ScriptableObject
{
    public int money;
    public int health;

    // combat
    public AttackSO[] attacks;
    public DashSO dash;

    // sound
    public FMODUnity.EventReference onHitSound;
    public FMODUnity.EventReference onDeathSound;

}
