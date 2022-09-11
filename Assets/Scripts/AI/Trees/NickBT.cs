using BehaviourTree;
using System.Collections.Generic;

/// <summary>
/// Behavioral tree for a mob from the first level.
/// </summary>
public class NickBT : CharacterTreeBase
{
    private int baseAttackID = 0;

    protected override void Init()
    {
        // PATROLLING
        Node patrollBT = GetPatrollBT();

        // ATTACKING  
        MeleeAttack meleeAttack = Character.GetAttackByID(baseAttackID) as MeleeAttack;

        // PC in LOS ?
        Node pcInRange = new FindTargetInRange(this, AIUtility.PCPositionName, (Character.Data as EnemyCharacterSO).lineOfSightRange, debugName: "pc in (FOV) range");

        // In Melee or Flurry Range ?
        Node meleeSequence = GetAttackBT(meleeAttack);

        // In Dash Range ?
        Node dashSequence = GetDashAttackBT(meleeAttack);

        // Walk
        Node walk = new WalkToTarget(this, AIUtility.PCPositionName, debugName: "walk to pc");

        // Attack tree
        Node attackSelector = new Selector(
            new List<Node>()
            {
                meleeSequence, dashSequence, walk
            }
        );
        Node attackBT = new Sequence(
            new List<Node>()
            {
                pcInRange, attackSelector
            }
        );

        // ROOT
        Root = new Selector(
            new List<Node>()
            {
                attackBT, patrollBT
            }
        );

    }
}
