using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "ScriptableObjects/AttackScriptableObject")]
public class AttackScriptableObject : ScriptableObject
{
    public string attackName;
    public int id;

    public EAttackButton button; // for player attacks only
    public EAttackEffect effect;

    [Range(1, 10)] public int damage;
    
    // min wait time between attacks
    public float delta;
    [Range(0, 1)] public float movementSpeedFactor = 0;

    // for ranged only
    public ProjectileController projectilePrefab;
    public float projectileSpeed = 100;
    [Range(1, 10)] public float projectileRange = 5; // in Unity units

    // TODO - return more attack classes, after they're designed and I create them
    public Attack GetAttack(AggressiveCharacter character)
    {
        Attack attack = null;

        switch (button)
        {
            case EAttackButton.Melee:
                attack = new MeleeAttack(character, this);
                break;
            case EAttackButton.Ranged:
                attack = new AimRangedAttack(character, this);
                break;
            case EAttackButton.Special:
                break;
            case EAttackButton.NDEF:
                break;
            default:
                break;
        }

        return attack;
    }
}
