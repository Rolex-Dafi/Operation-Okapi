using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackBT : CharacterTreeBase
{
    protected override void Init()
    {
        Debug.Log("From ranged attack bt, player position is " + playerCharacter.position);

        Node findTarget = new TargetInLineOfSight(rootTree, playerCharacter, (Character.CharacterData as EnemyCharacterSO).lineOfSightRange);

        // TODO replace reload time with attack reference later ?
        Node attackTarget = new RangedAttackTarget(rootTree, playerCharacter, (Character.CharacterData as EnemyCharacterSO).reloadTime);

        Root = new Sequence(new List<Node> { findTarget, attackTarget });
    }
}
