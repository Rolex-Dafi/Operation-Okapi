using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrollBT))]
public class DummyBT : CharacterTreeBase
{

    protected override void Init(TreeBase rootTree = null)
    {
        Root = GetComponent<PatrollBT>().Root;
    }

}
