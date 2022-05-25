using UnityEngine;

/// <summary>
/// Base wrapper class for all ranged attacks.
/// </summary>
public class AimRangedAttack : RangedAttack
{
    private PlayerController playerController;

    private new PlayerCharacter character;

    public AimRangedAttack(AggressiveCharacter character, ProjectileController projectilePrefab) : base(character, projectilePrefab)
    {
        this.character = (PlayerCharacter)character;
    }

    public AimRangedAttack(int attackID, AggressiveCharacter character, ProjectileController projectilePrefab) : 
        base(attackID, character, projectilePrefab)
    {
        this.character = (PlayerCharacter)character;
    }

    public override void OnBegin()
    {
        // TODO replace by start animation -> changes in Animator neccessary
        base.OnBegin();
        character.StartAiming();
    }

    public override void OnEnd()
    {
        // TODO stop animation -> changes in Animator neccessary
        character.StopAiming();
        SpawnProjectile();
    }
}
