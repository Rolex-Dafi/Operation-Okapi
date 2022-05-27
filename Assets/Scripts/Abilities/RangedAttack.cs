using UnityEngine;

/// <summary>
/// Base wrapper class for all ranged attacks.
/// </summary>
public class RangedAttack : Attack
{
    protected Vector2 target;

    // projectile
    private ProjectileController projectilePrefab;
    private Transform projectileSpawnerTransform;
    private float projectileSpeed;
    private float projectileRange;

    public Transform ProjectileSpawnerTransform { get => projectileSpawnerTransform; set => projectileSpawnerTransform = value; }
    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public Vector2 Target { get => target; set => target = value; }
    public float ProjectileRange { get => projectileRange; set => projectileRange = value; }

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
        //this.projectilePrefab.Init(Damage, projectileSpeed);
    }

    protected void SpawnProjectile()
    {
        // add a small constant to radius to spawn it outside of the character collider
        // shoot at target - asumes target is set beforehand !!
        Debug.Log("shooting at target " + target);
        Vector2 direction = (target - projectileSpawnerTransform.position.ToVector2()).normalized;
        Vector2 spawnOffset = (character.ColliderRadius + .1f) * direction;
        ProjectileController instance = Object.Instantiate(
            projectilePrefab,
            ProjectileSpawnerTransform.position + spawnOffset.ToVector3(),
            Quaternion.identity, 
            ProjectileSpawnerTransform
        );
        // Player and enemies have to be tagged correctly for this to work !
        instance.Init(Damage, projectileSpeed, projectileRange, character.tag);
        instance.Shoot(direction);
    }
}
