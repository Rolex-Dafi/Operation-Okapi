
/// <summary>
/// Base wrapper class for all ranged attacks.
/// </summary>
public class RangedAttack : Attack
{
    public RangedAttack(AggressiveCharacter character) : base(character, 1)
    {
        attackType = EAttackType.Ranged;
    }

    public RangedAttack(AggressiveCharacter character, int attackNumber) : base(character, attackNumber)
    {
        attackType = EAttackType.Ranged;
    }
}
