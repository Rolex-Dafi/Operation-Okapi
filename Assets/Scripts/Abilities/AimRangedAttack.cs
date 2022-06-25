using UnityEngine;

/// <summary>
/// Class for ranged attacks aimed by the player.
/// </summary>
public class AimRangedAttack : RangedAttack
{
    // only player can perform aim attacks
    private new PlayerCharacter character;

    public AimRangedAttack(CombatCharacter character, AttackScriptableObject data) : base(character, data)
    {
        this.character = (PlayerCharacter)character;
    }

    public override void OnBegin()
    {
        base.OnBegin();
        character.StartAiming();
    }

    public override void OnEnd()
    {
        character.Animator.SetTrigger(EAnimationParameter.attackReleased.ToString());
        character.StopAiming();

        SpawnProjectile();
    }
}
