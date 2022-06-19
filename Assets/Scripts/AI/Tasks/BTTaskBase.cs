using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;
using UnityEngine.Events;

public abstract class BTTaskBase : Leaf
{
    protected BTTreeBase bt;

    private bool taskInProgress = false;

    protected BTTaskBase(BTTreeBase characterBT) : base()
    {
        bt = characterBT;
    }

    public override NodeStatus Update()
    {
        if (!taskInProgress)
        {
            taskInProgress = true;
            status = NodeStatus.Running;
            OnBegin();
        }
        else
        {
            status = NodeStatus.Running;
            OnContinue();
        }

        return status;
    }

    protected abstract void OnBegin();

    protected abstract void OnContinue();

    /// <summary>
    /// Every task has to call this after finishing !
    /// </summary>
    /// <param name="taskSuccess"></param>
    protected void OnEnd(bool taskSuccess)
    {
        taskInProgress = false;
        status = taskSuccess ? NodeStatus.Success : NodeStatus.Failure;
    }
}
