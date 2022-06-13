using UnityEngine;

/// <summary>
/// Base wrapper class for all ranged attacks.
/// </summary>
public class RangedAttack : Attack
{
    protected Vector2 target;

    public Vector2 Target { get => target; set => target = value; }

    public RangedAttack(AggressiveCharacter character, AttackScriptableObject data) : base(character , data) { }


    protected void SpawnProjectile()
    {
        // add a small constant to radius to spawn it outside of the character collider
        // shoot at target - asumes target is set beforehand !!
        Vector2 direction = (target - character.ProjectileSpawnerTransform.position.ToVector2()).normalized;
        Vector2 spawnOffset = (character.ColliderRadius + .1f) * direction;
        ProjectileController instance = Object.Instantiate(
            data.projectilePrefab,
            character.ProjectileSpawnerTransform.position + spawnOffset.ToVector3(),
            Quaternion.identity,
            character.ProjectileSpawnerTransform
        );
        // Player and enemies have to be tagged correctly for this to work !
        instance.Init(data.damage, data.projectileSpeed, data.projectileRange, character.tag);
        instance.Shoot(direction);
    }
}
