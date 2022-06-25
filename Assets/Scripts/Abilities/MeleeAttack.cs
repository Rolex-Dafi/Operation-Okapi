
/// <summary>
/// Base wrapper class for all melee attacks.
/// </summary>
public class MeleeAttack : Attack
{
    private HitBoxController hitBoxController;

    public MeleeAttack(CombatCharacter character, AttackScriptableObject data) : base(character, data) 
    {
        hitBoxController = character.GetComponentInChildren<HitBoxController>();
    }

    public override void OnBegin()
    {
        base.OnBegin();
        hitBoxController.Init(data.damage);
    }

}
