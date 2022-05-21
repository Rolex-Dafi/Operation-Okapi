using System;
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

    // for ranged only
    public ProjectileController projectilePrefab;
    public float projectileSpeed = 100;

    // TODO - return more attack classes, after they're designed and I create them
    public Attack GetAttack(AggressiveCharacter character)
    {
        Attack attack = null;

        switch (attackButton)
        {
            case EAttackButton.Melee:
                attack = new MeleeAttack(attackID, character)
                {
                    Damage = damage,
                    AttackEffect = attackEffect,
                    MovementSpeedFactor = movementSpeedFactor
                };
                break;
            case EAttackButton.Ranged:
                attack = new AimRangedAttack(attackID, character, projectilePrefab)
                {
                    Damage = damage,
                    AttackEffect = attackEffect,
                    MovementSpeedFactor = movementSpeedFactor,
                    ProjectileTransform = character.ProjectileTransform,
                    ProjectileSpeed = projectileSpeed
                };
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
