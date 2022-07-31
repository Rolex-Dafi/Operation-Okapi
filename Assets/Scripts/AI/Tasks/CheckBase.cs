using BehaviourTree;
using UnityEngine;

/// <summary>
/// Base class for all checks. Checks only check whether this branch is valid.
/// </summary>
public abstract class CheckBase : Leaf
{
    protected CharacterTreeBase bt;

    protected CheckBase(CharacterTreeBase characterBT, string debugName = "") : base(debugName)
    {
        bt = characterBT;
    }

    public override NodeStatus Update()
    {
        // ! assumes check is fast enough to be performed in one tick !
        status = Check() ? NodeStatus.Success : NodeStatus.Failure;

        //Debug.Log("Finished check " + ToString() + ", check status: " + status);

        return status;
    }

    protected abstract bool Check();
}
