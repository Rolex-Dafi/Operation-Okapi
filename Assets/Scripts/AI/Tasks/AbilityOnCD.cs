
public class AbilityOnCD : CheckBase
{
    private Ability ability;

    public AbilityOnCD(CharacterTreeBase characterBT, Ability ability, string debugName = "") : base(characterBT, debugName)
    {
        this.ability = ability;
    }

    protected override bool Check()
    {
        return ability.OnCoolDown;
    }
}
