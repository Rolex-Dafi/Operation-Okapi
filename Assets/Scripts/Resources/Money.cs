/// <summary>
/// Wrapper class for the money resource. For the player and merchant NPC it should represent their money amount, 
/// for aggressive NPCs the amount of money they drop.
/// </summary>
public class Money : Resource
{
    public Money(int startingValue) : base(startingValue, int.MaxValue) { }
}
