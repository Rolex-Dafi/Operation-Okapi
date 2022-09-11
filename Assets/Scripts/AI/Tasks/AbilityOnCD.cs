/// <summary>
/// Checks whether an ability is on cool down.
/// </summary>
public class AbilityOnCD : CheckBase
{
    private Ability ability;

    /// <summary>
    /// Creates a task instance.
    /// </summary>
    /// <param name="characterBT">The behavioral tree of this character</param>
    /// <param name="ability">The ability to check</param>
    /// <param name="debugName"></param>
    public AbilityOnCD(CharacterTreeBase characterBT, Ability ability, string debugName = "") : base(characterBT, debugName)
    {
        this.ability = ability;
    }

    protected override bool Check()
    {
        return ability.OnCoolDown;
    }
}
