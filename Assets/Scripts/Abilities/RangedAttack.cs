using UnityEngine;

/// <summary>
/// Base wrapper class for all ranged attacks.
/// </summary>
public class RangedAttack : Attack
{
    // projectile
    private ProjectileController projectilePrefab;
    private Transform projectileTransform;
    private float projectileSpeed;

    public Transform ProjectileTransform { get => projectileTransform; set => projectileTransform = value; }
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
        ProjectileController instance = GameObject.Instantiate(projectilePrefab, ProjectileTransform);
        instance.Init(Damage);
        instance.Shoot(character.Facing.ToVector2() * projectileSpeed);
    }
}
