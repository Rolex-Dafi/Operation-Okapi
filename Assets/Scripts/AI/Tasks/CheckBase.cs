using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all checks. Checks only check whether this branch is valid.
/// </summary>
public abstract class CheckBase : Leaf
{
    protected CharacterTreeBase bt;

    protected CheckBase(CharacterTreeBase characterBT) : base()
    {
        bt = characterBT;
    }

    public override NodeStatus Update()
    {
        Debug.LogWarning("Update in check node " + this);

        status = Check() ? NodeStatus.Success : NodeStatus.Failure;

        return status;
    }

    protected abstract bool Check();
}
