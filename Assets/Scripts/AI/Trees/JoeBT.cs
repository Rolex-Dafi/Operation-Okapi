using BehaviourTree;
using System.Collections.Generic;

/// <summary>
/// Behavioral tree for a mob from the second level.
/// </summary>
public class JoeBT : CharacterTreeBase
{
    protected override void Init()
    {
        // PATROLLING
        Node patrollBT = GetPatrollBT();

        // PC in LOS ?
        Node pcInRange = new FindTargetInRange(this, AIUtility.PCPositionName, (Character.Data as EnemyCharacterSO).lineOfSightRange, debugName: "pc in (FOV) range");

        // In Dash Range ?
        Node dashSequence = GetDashBT(true);

        // Walk
        Node walk = new WalkToTarget(this, AIUtility.PCPositionName, debugName: "walk to pc");

        // Attack tree
        Node attackSelector = new Selector(
            new List<Node>()
            {
                dashSequence, walk
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
