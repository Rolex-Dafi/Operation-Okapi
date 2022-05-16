using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "ScriptableObjects/AttackScriptableObject")]
public class AttackScriptableObject : ScriptableObject
{
    public string attackName;
    public int attackID;

    public EAttackButton attackButton; // for player attacks only
    public EAttackEffect attackEffect;

    [Range(1, 10)] public int damage;
    [Range(0, 1)] public float movementSpeedFactor = 0;

    public GameObject projectilePrefab; // for ranged only
}
