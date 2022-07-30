using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackBT : CharacterTreeBase
{
    [SerializeField] protected int rangedAttackId = 1;

    protected override void Init()
    {
        Debug.Log("RangedAttackBT initiated");
        Node findTarget = new EntityInLineOfSight(rootTree, playerCharacter, "lastPCPosition", (Character.Data as EnemyCharacterSO).lineOfSightRange);

        // TODO add if trgt in attack range - if not, move or dash tw it

        Node attackTarget = new AttackEntity(rootTree, playerCharacter, Character.GetAttackByID(rangedAttackId));

        Root = new Sequence(new List<Node> { findTarget, attackTarget });
    }
}
