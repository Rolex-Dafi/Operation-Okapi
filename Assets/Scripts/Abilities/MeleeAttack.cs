
/// <summary>
/// Base wrapper class for all melee attacks.
/// </summary>
public class MeleeAttack : Attack
{
    public MeleeAttack(AggressiveCharacter character) : base(character, 0)
    {
    }

    public MeleeAttack(AggressiveCharacter character, int attackNumber) : base(character, attackNumber)
    {
    }
}
