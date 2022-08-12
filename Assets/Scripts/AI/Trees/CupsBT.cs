using BehaviourTree;
using System.Collections.Generic;

public class CupsBT : CharacterTreeBase
{
    private int slapAttackID = 0;
    private int baseAttackID = 1;

    protected override void Init()
    {
        // PATROLLING
        Node patrollBT = GetPatrollBT();

        // ATTACKING  
        RangedAttack rangedAttack = Character.GetAttackByID(baseAttackID) as RangedAttack;
        MeleeAttack slapAttack = Character.GetAttackByID(slapAttackID) as MeleeAttack;

        // PC in LOS ?
        Node pcInRange = new FindTargetInRange(this, AIUtility.PCPositionName, (Character.Data as EnemyCharacterSO).lineOfSightRange, debugName: "pc in (FOV) range");

        // In Slap Range ?
        Node slapSequence = GetAttackBT(slapAttack);

        // In Ranged Range ?
        Node rangedSequence = GetAttackBT(rangedAttack);

        // Walk if not in ranged range
        Node walk = new WalkToTarget(this, AIUtility.PCPositionName, debugName: "walk to pc");

        // TODO walk away from pc to a safe distance if too close
        // make a new task node for this - keep distance from target

        // Attack tree
        Node attackSelector = new Selector(
            new List<Node>()
            {
                slapSequence, rangedSequence, walk
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
