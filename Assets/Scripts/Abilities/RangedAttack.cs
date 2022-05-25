using UnityEngine;

/// <summary>
/// Base wrapper class for all ranged attacks.
/// </summary>
public class RangedAttack : Attack
{
    // projectile
    private ProjectileController projectilePrefab;
    private Transform projectileSpawnerTransform;
    private float projectileSpeed;

    public Transform ProjectileSpawnerTransform { get => projectileSpawnerTransform; set => projectileSpawnerTransform = value; }
    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }

    public RangedAttack(AggressiveCharacter character, ProjectileController projectilePrefab) : base(1, character)
    {
        Init(projectilePrefab);
    }

    public RangedAttack(int attackID, AggressiveCharacter character, ProjectileController projectilePrefab) : 
        base(attackID, character)
    {
        Init(projectilePrefab);
    }

    private void Init(ProjectileController projectilePrefab)
    {
        this.projectilePrefab = projectilePrefab;
        this.projectilePrefab.Init(Damage);
    }

    protected void SpawnProjectile()
    {
        // add a small constant to radius to spawn it outside of the character collider
        Vector2 spawnOffset = (character.ColliderRadius + .1f) * character.Facing.CartesianToIsometric().normalized;
        ProjectileController instance = Object.Instantiate(
            projectilePrefab,
            ProjectileSpawnerTransform.position + spawnOffset.ToVector3(),
            Quaternion.identity, 
            ProjectileSpawnerTransform
        );
        instance.Init(Damage);
        instance.Shoot(character.Facing * projectileSpeed);
    }
}
