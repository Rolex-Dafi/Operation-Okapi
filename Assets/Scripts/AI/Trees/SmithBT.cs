using BehaviourTree;
using System.Collections.Generic;

public class SmithBT : CharacterTreeBase
{
    private int roarAttackID = 0;
    private int heavyStepsAttackID = 1;

    protected override void Init()
    {
        // PATROLLING
        Node patrollBT = GetPatrollBT();

        // ATTACKING  
        RangedAttack stepsAttack = Character.GetAttackByID(heavyStepsAttackID) as RangedAttack;
        MeleeAttack roarAttack = Character.GetAttackByID(roarAttackID) as MeleeAttack;

        // PC in LOS ?
        Node pcInRange = new FindTargetInRange(this, AIUtility.PCPositionName, (Character.Data as EnemyCharacterSO).lineOfSightRange, debugName: "pc in (FOV) range");

        // In Roar Range and not on CD ?
        Node roarSequence = GetAttackBT(roarAttack, true);

        // In Heavy Steps Range and not on CD?
        Node stepsSequence = GetAttackBT(stepsAttack, true);

        // Walk if not in ranged range
        Node walk = new WalkToTarget(this, AIUtility.PCPositionName, debugName: "walk to pc");

        // TODO walk away from pc to a safe distance if too close
        // make a new task node for this - keep distance from target

        // Attack tree
        Node attackSelector = new Selector(
            new List<Node>()
            {
                roarSequence, stepsSequence, walk
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
