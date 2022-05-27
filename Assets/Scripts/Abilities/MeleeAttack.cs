
/// <summary>
/// Base wrapper class for all melee attacks.
/// </summary>
public class MeleeAttack : Attack
{
    public MeleeAttack(AggressiveCharacter character) : base(0, character)
    {
    }

    public MeleeAttack(int attackID, AggressiveCharacter character) : base(attackID, character)
    {
    }
}
