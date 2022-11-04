
/// <summary>
/// Base wrapper class for all melee attacks.
/// </summary>
public class MeleeAttack : Attack
{
    private HitBoxController hitBoxController;

    /// <summary>
    /// Creates a melee attack instance.
    /// </summary>
    /// <param name="character">The character to which the attack belongs</param>
    /// <param name="data">The attack data</param>
    public MeleeAttack(CombatCharacter character, AttackSO data) : base(character, data, EAbilityType.melee) 
    {
        hitBoxController = character.GetComponentInChildren<HitBoxController>();
    }

    public override void OnBegin()
    {
        if (Target != null)
        {
            // rotate tw target
            character.Rotate((Target.GetValueOrDefault() - character.transform.position).normalized);
        }
        
        base.OnBegin();
        hitBoxController.Init(Data);
    }

}
