using BehaviourTree;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrollBT), typeof(RangedAttackBT))]
public class DummyBT : CharacterTreeBase
{
    protected override void Init()
    {
        Debug.Log("DummyBT initiated");
        Root = new Selector(
            new List<Node> {
                GetComponent<RangedAttackBT>().Root,
                GetComponent<PatrollBT>().Root
            }
        );

    }

}
