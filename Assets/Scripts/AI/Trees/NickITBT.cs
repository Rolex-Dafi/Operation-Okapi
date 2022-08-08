using BehaviourTree;
using System.Collections.Generic;

public class NickITBT : CharacterTreeBase
{
    private readonly int baseAttackID = 0;
    private readonly int flurryAttackID = 2;

    protected override void Init()
    {
        // PATROLLING
        Node patrollBT = GetPatrollBT();

        // ATTACKING  
        MeleeAttack meleeAttack = Character.GetAttackByID(baseAttackID) as MeleeAttack;
        MeleeAttack flurryAttack = Character.GetAttackByID(flurryAttackID) as MeleeAttack;

        // PC in LOS ?
        Node pcInRange = new FindTargetInRange(this, AIUtility.PCPositionName, (Character.Data as EnemyCharacterSO).lineOfSightRange, debugName: "pc in (FOV) range");

        // In Melee or Flurry Range ?
        Node meleeSequence = GetAttackBT(meleeAttack); 
        Node flurrySequence = GetAttackBT(flurryAttack, true); 

        // In Dash Range ?
        Node dashSequence = GetDashAttackBT(meleeAttack);

        // Walk
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
    }
}
