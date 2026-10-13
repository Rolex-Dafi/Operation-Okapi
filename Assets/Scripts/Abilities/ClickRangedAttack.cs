/// <summary>
/// Class for a basic ranged attack.
/// </summary>
public class ClickRangedAttack : RangedAttack
{
    /// <summary>
    /// Creates an instance of a click ranged attack - a ranged attack which shoots the projectile on button down.
    /// </summary>
    /// <param name="character">The character to which the attack belongs</param>
    /// <param name="data">The attack data</param>
    public ClickRangedAttack(CombatCharacter character, AttackSO data) : base(character, data) { }

    public override void OnBegin()
    {
        base.OnBegin();
        SpawnProjectile(); // TODO invoke after delay specified in data
        //character.Invoke("SpawnProjectile", (data as AttackSO).projectileDelay);
    }

}
