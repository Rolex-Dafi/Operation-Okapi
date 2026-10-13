/// <summary>
/// Wrapper class for the respect resource. Exclusive to the player, represents the current respect value.
/// </summary>
public class Respect : Resource
{
    public Respect(int startingValue) : base(startingValue, int.MaxValue) { }
}
