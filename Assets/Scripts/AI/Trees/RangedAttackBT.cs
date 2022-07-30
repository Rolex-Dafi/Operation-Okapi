using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackBT : CharacterTreeBase
{
    [SerializeField] protected int rangedAttackId = 1;

    protected override void Init()
    {
        Node findTarget = new TargetInLineOfSight(rootTree, playerCharacter, (Character.Data as EnemyCharacterSO).lineOfSightRange);

        // TODO add if trgt in attack range - if not, move or dash tw it

        Node attackTarget = new AttackTarget(rootTree, playerCharacter, Character.GetAttackByID(rangedAttackId));

        Root = new Sequence(new List<Node> { findTarget, attackTarget });
    }
}
