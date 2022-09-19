/// <summary>
/// Wrapper class for the health resource. Represents the current and max health
/// for all characters that can engage in combat.
/// </summary>
public class Health : Resource
{
    public Health(int maxValue) : base(maxValue, maxValue) { }

}
