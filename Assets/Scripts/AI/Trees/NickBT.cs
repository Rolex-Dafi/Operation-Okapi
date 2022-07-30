using BehaviourTree;
using System.Collections.Generic;
using UnityEngine;

public class NickBT : CharacterTreeBase
{
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
        Node patrollBT = new Sequence(patrollTasks);

        // ATTACKING  
        MeleeAttack meleeAttack = Character.GetAttackByID(0) as MeleeAttack;
        // PC in LOS ?
        // TODO currently only checks range, not whether the target is obstructed
        Node pcInRange = new FindTargetInRange(this, AIUtility.PCPositionName, (Character.Data as EnemyCharacterSO).lineOfSightRange);
        // In Melee Range ?
        Node meleeRange = new Sequence(
            new List<Node>() 
            {
                new TargetInRange(this, AIUtility.PCPositionName, meleeAttack.Data.attackRange),
                new AttackTarget(this, meleeAttack, AIUtility.PCPositionName),
                new WaitFor(this, meleeAttack.Data.recoveryTime)
            }
        );
        // In Dash Range ?
        Node dashRange = new Sequence(
            new List<Node>()
            {
                new TargetInRange(this, AIUtility.PCPositionName, Character.GetDash().Data.distance),
                new DashToTarget(this, AIUtility.PCPositionName),
                new AttackTarget(this, meleeAttack, AIUtility.PCPositionName),
                new WaitFor(this, meleeAttack.Data.recoveryTime)
            }
        );
        // Walk 
        Node walk = new WalkToTarget(this, AIUtility.PCPositionName);

        // Attack tree
        Node attackSelector = new Selector(
            new List<Node>()
            {
                meleeRange, dashRange, walk
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
