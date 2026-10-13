
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

/// <summary>
/// Behavioral tree for the boss of the last level.
/// </summary>
public class BridgetBT : CharacterTreeBase
{
    private int baseAttackID = 1;

    protected override void Init()
    {
        // ATTACKING  
        RangedAttack rangedAttack = Character.GetAttackByID(baseAttackID) as RangedAttack;
        
        // PC in LOS ?
        var pcInRange = new FindTargetInRange(this, AIUtility.PCPositionName, (Character.Data as EnemyCharacterSO).lineOfSightRange);
        // always attack whenever you can
        var attackBT = GetAttackBT(rangedAttack, true, true);

        Root = new Sequence(new List<Node>() { pcInRange, attackBT });

    }
}