
/// <summary>
/// Base wrapper class for all ranged attacks.
/// </summary>
public class AimRangedAttack : RangedAttack
{
    public AimRangedAttack(AggressiveCharacter character) : base(character)
    {
    }

    public AimRangedAttack(AggressiveCharacter character, int attackNumber) : base(character, attackNumber)
    {
    }

    public override void OnBegin()
    {

    }
}
