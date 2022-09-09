using BehaviourTree;
using System.Collections.Generic;

public class JimmyBT : CharacterTreeBase
{
    private int baseAttackID = 1;

    protected override void Init()
    {
        // PATROLLING
        Node patrollBT = GetPatrollBT();

        // ATTACKING  
        RangedAttack rangedAttack = Character.GetAttackByID(baseAttackID) as RangedAttack;
        Trap fountain = Character.GetMainTrap();

        // PC in LOS ?
        Node pcInRange = new FindTargetInRange(this, AIUtility.PCPositionName, (Character.Data as EnemyCharacterSO).lineOfSightRange, debugName: "pc in (FOV) range");

        // Fountain on cd ?
        Node fountainSequence = GetTrapBT(fountain);
        
        // In Ranged Range ?
        Node rangedSequence = GetAttackBT(rangedAttack, true);

        // Walk if not in ranged range
        Node walk = new WalkToTarget(this, AIUtility.PCPositionName, debugName: "walk to pc");

        // Attack tree
        Node attackSelector = new Selector(
            new List<Node>()
            {
                fountainSequence, rangedSequence, walk
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
