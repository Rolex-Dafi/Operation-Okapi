
/// <summary>
/// Base wrapper class for all melee attacks.
/// </summary>
public class MeleeAttack : Attack
{
    private HitBoxController hitBoxController;

    public MeleeAttack(CombatCharacter character, AttackSO data) : base(character, data, EAbilityType.melee) 
    {
        hitBoxController = character.GetComponentInChildren<HitBoxController>();
    }

    public override void OnBegin()
    {
        base.OnBegin();
        hitBoxController.Init(Data.damage);
    }

}
