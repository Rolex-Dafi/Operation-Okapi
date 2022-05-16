using UnityEngine;

/// <summary>
/// Base wrapper class for all ranged attacks.
/// </summary>
public class RangedAttack : Attack
{
    protected ProjectileController projectilePrefab;

    protected Vector2 direction;

    public RangedAttack(AggressiveCharacter character, ProjectileController projectilePrefab) : base(character, 1)
    {
        Init(projectilePrefab);
    }

    public RangedAttack(AggressiveCharacter character, ProjectileController projectilePrefab, int attackNumber) : base(character, attackNumber)
    {
        Init(projectilePrefab);
    }

    private void Init(ProjectileController projectilePrefab)
    {
        this.projectilePrefab = projectilePrefab;
        this.projectilePrefab.Init(damage);
    }
}
