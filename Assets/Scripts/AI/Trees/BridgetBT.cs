using BehaviourTree;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class BridgetBT : CharacterTreeBase
{
    private int baseAttackID = 1;

    protected override void Init()
    {
        // ATTACKING  
        RangedAttack rangedAttack = Character.GetAttackByID(baseAttackID) as RangedAttack;

        // always attack whenever you can
        Root = GetAttackBT(rangedAttack, true, true);

    }
}