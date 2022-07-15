using UnityEngine;

/// <summary>
/// Base wrapper class for all ranged attacks.
/// </summary>
public class RangedAttack : Attack
{
    public RangedAttack(CombatCharacter character, AttackSO data) : base(character , data, EAbilityType.ranged) { }

    protected void SpawnProjectile()
    {
        // add a small constant to radius to spawn it outside of the character collider
        Vector2 spawnOffset = (character.ColliderRadius + .1f) * character.Facing;
        ProjectileController instance = Object.Instantiate(
            Data.projectilePrefab,
            character.ProjectileSpawnerTransform.position + spawnOffset.ToVector3(),
            Quaternion.identity,
            character.ProjectileSpawnerTransform
        );
        // Player and enemies have to be tagged correctly for this to work !
        instance.Init(Data.damage, Data.projectileSpeed, Data.projectileRange, character.tag);
        instance.Shoot(character.Facing);
    }
}
