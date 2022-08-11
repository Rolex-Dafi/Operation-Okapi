using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "ScriptableObjects/Attack")]
public class AttackSO : AbilitySO
{
    public string attackName;
    public int id;

    public EAttackButton button; // for player attacks only
    public EAttackEffect effect;

    [Range(0, 10)] public int damage;

    [Range(0, 10)] public int cost; // for player attacks only, enemies should (?) have unlimited attacks
    
    // min wait time between attacks
    // TODO actually use this somewhere for player (enemies already use it)
    public float recoveryTime = 1f;

    [Range(0, 10)] public float attackRange = 3; // in Unity units

    // how much is the character slown during the attack (0..completely, 1..full speed)
    [Range(0, 1)] public float movementSpeedFactor = 0;

    public float enemyPushbackDistance = 0;
    public float enemyPushbackSpeed = 40;

    // for ranged only
    public ProjectileController projectilePrefab;
    public float projectileSpeed = 100;

    // TODO - return more attack classes, after they're designed and I create them
    public Attack GetAttack(CombatCharacter character)
    {
        Attack attack = null;

        switch (button)
        {
            case EAttackButton.Melee:
                attack = new MeleeAttack(character, this);
                break;
            case EAttackButton.Ranged:
                {
                    switch (effect)
                    {
                        case EAttackEffect.Click:
                            attack = new ClickRangedAttack(character, this);
                            break;
                        case EAttackEffect.Aim:
                            attack = new AimRangedAttack(character, this);
                            break;
                    }
                }
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
