using BehaviourTree;
using System.Collections.Generic;

public class PottsBT : CharacterTreeBase
{
    private int baseAttackID = 1;

    protected override void Init()
    {
        // PATROLLING
        // TODO set patroll points from here -> for each dungeon from the navmesh/graph
        List<Node> patrollTasks = new List<Node>();
        for (int i = 0; i < patrollPoints.Length; i++)
        {
            patrollTasks.Add(new WalkToTarget(rootTree, patrollPoints[i].position));

            if (i < patrollPoints.Length - 1) patrollTasks.Add(new WaitFor(rootTree, (Character.Data as EnemyCharacterSO).patrollWaitTime));
        }
        Node patrollBT = new SequenceWithCachedLastChild(patrollTasks);

        // ATTACKING  
        RangedAttack rangedAttack = Character.GetAttackByID(baseAttackID) as RangedAttack;
        // PC in LOS ?
        // TODO currently only checks range, not whether the target is unobstructed
        Node pcInRange = new FindTargetInRange(this, AIUtility.PCPositionName, (Character.Data as EnemyCharacterSO).lineOfSightRange, debugName: "pc in (FOV) range");

        // In Ranged Range ?
        Node rangedTask = new SequenceWithCachedLastChild(
            new List<Node>()
            {
                new AttackTarget(this, rangedAttack, AIUtility.PCPositionName, debugName:"attack pc"),                            // attack pc THEN
                new WaitFor(this, rangedAttack.Data.recoveryTime, debugName:"attack recovery")                                    // attack recovery 
            }
        );
        Node rangedSequence = new Sequence(
            new List<Node>()
            {
                new TargetInRange(this, AIUtility.PCPositionName, rangedAttack.Data.attackRange, debugName:"pc in atk range"),     // pc in attack range AND
                rangedTask                                                                                                         // perform atk task
            }
        );

        // Walk if not in ranged range
        Node walk = new WalkToTarget(this, AIUtility.PCPositionName, debugName: "walk to pc");

        // Attack tree
        Node attackSelector = new Selector(
            new List<Node>()
            {
                rangedSequence, walk
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
