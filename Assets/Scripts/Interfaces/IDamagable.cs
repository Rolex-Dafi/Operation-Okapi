/// <summary>
/// Any damageable entity should implement this.
/// </summary>
public interface IDamagable
{
    /// <summary>
    /// Deal damage to this damageable.
    /// </summary>
    /// <param name="amount">The amount of damage to deal</param>
    public void TakeDamage(int amount);

    public void Die();
}
