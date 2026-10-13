using FMODUnity;
using UnityEngine;

/// <summary>
/// Class for ranged attacks aimed by the player.
/// </summary>
public class AimRangedAttack : RangedAttack
{
    // only player can perform aim attacks
    private new PlayerCharacter character;

    /// <summary>
    /// Creates a new aimed ranged attack instance - a ranged attack the player can aim with and which shoots the
    /// projectile on button release.
    /// </summary>
    /// <param name="character">The character to which the attack belongs</param>
    /// <param name="data">The attack data</param>
    public AimRangedAttack(CombatCharacter character, AttackSO data) : base(character, data)
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

        // play sound after releasing the attack
        if (!data.onEndSound.IsNull) RuntimeManager.PlayOneShot(data.onEndSound.Guid);
        SpawnProjectile();
    }
}
