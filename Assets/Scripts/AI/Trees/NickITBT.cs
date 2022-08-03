using BehaviourTree;
using System.Collections.Generic;

public class NickITBT : CharacterTreeBase
{
    private int baseAttackID = 0;
    private int flurryAttackID = 2;

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
        MeleeAttack meleeAttack = Character.GetAttackByID(baseAttackID) as MeleeAttack;
        MeleeAttack flurryAttack = Character.GetAttackByID(flurryAttackID) as MeleeAttack;
        // PC in LOS ?
        // TODO currently only checks range, not whether the target is unobstructed
        Node pcInRange = new FindTargetInRange(this, AIUtility.PCPositionName, (Character.Data as EnemyCharacterSO).lineOfSightRange, debugName: "pc in (FOV) range");

        // In Melee Range ?
        Node meleeTask = new SequenceWithCachedLastChild(
            new List<Node>()
            {
                new AttackTarget(this, meleeAttack, AIUtility.PCPositionName, debugName:"attack pc"),                            // attack pc THEN
                new WaitFor(this, meleeAttack.Data.recoveryTime, debugName:"attack recovery")                                    // attack recovery 
            }
        );
        Node flurryTask = new SequenceWithCachedLastChild(
            new List<Node>()
            {
                new AttackTarget(this, flurryAttack, AIUtility.PCPositionName, debugName:"flurry attack pc"),                            // attack pc THEN
                new WaitFor(this, flurryAttack.Data.recoveryTime, debugName:"flurry recovery")                                    // attack recovery 
            }
        );
        Node meleeSequence = new Sequence(
            new List<Node>()
            {
                new TargetInRange(this, AIUtility.PCPositionName, meleeAttack.Data.attackRange, debugName:"pc in atk range"),     // pc in attack range AND
                meleeTask                                                                                                         // perform atk task
            }
        );
        Node flurrySequence = new Sequence(
            new List<Node>()
            {
                new Inverter(new AbilityOnCD(this, flurryAttack, debugName:"flurry on cd")),                                         // flurry not on cd AND  
                new TargetInRange(this, AIUtility.PCPositionName, flurryAttack.Data.attackRange, debugName:"pc in flurry range"),     // pc in attack range AND
                flurryTask                                                                                                         // perform atk task
            }
        );

        // In Dash Range ?
        Node dashTask = new SequenceWithCachedLastChild(
            new List<Node>()
            {
                //new DashToTarget(this, AIUtility.PCPositionName, debugName:"dash to pc"),                                         //// dash to pc THEN
                new DashAttackTarget(this, meleeAttack, AIUtility.PCPositionName, debugName:"attack pc"),                           // dash attack pc THEN
                new WaitFor(this, meleeAttack.Data.recoveryTime, debugName:"attack recovery")                                     // attack recovery 
            }
        );
        Node dashSequence = new Sequence(
            new List<Node>()
            {
                new Inverter(new AbilityOnCD(this, Character.GetDash(), debugName:"dash on cd")),                                   // dash not on cd AND
                new TargetInRange(this, AIUtility.PCPositionName, Character.GetDash().Data.distance, debugName:"pc in dash range"), // pc at dash range AND
                dashTask                                                                                                            // perform dash
            }
        );

        // Walk if not in melee range
        Node walk = new WalkToTarget(this, AIUtility.PCPositionName, debugName: "walk to pc");

        // Attack tree
        Node attackSelector = new Selector(
            new List<Node>()
            {
                flurrySequence, meleeSequence, dashSequence, walk
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

        //Root = attackBT;


    }
}
