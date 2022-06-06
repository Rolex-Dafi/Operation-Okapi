using UnityEngine;

/// <summary>
/// Class for ranged attacks aimed by the player.
/// </summary>
public class AimRangedAttack : RangedAttack
{
    // only player can perform aim attacks
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
        character.animator.SetTrigger(EAnimationParameter.attackReleased.ToString());
        character.StopAiming();

        SpawnProjectile();
    }
}
