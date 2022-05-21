
/// <summary>
/// Base wrapper class for all ranged attacks.
/// </summary>
public class AimRangedAttack : RangedAttack
{
    public AimRangedAttack(AggressiveCharacter character, ProjectileController projectilePrefab) : base(character, projectilePrefab)
    {
    }

    public AimRangedAttack(int attackID, AggressiveCharacter character, ProjectileController projectilePrefab) : 
        base(attackID, character, projectilePrefab)
    {
    }

    // TODO
    public override void OnBegin()
    {
        base.OnBegin();
    }

    public override void OnEnd()
    {
        SpawnProjectile();
    }
}
