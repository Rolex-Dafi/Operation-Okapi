using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character data class.
/// </summary>
[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/Character")]
public class CharacterSO : ScriptableObject
{
    public int money;
    public int health;

    public float speed;
    
    // combat
    public AttackSO[] attacks;
    public TrapSO[] traps;
    public DashSO dash;

    // sound
    public FMODUnity.EventReference onHitSound;
    public FMODUnity.EventReference onDeathSound;

}
