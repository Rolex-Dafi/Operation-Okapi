using BehaviourTree;

/// <summary>
/// Base class for all tasks. Tasks update the character's animation, physics, etc.
/// </summary>
public abstract class TaskBase : Leaf
{
    protected CharacterTreeBase bt;

    private bool taskInProgress = false;

    protected TaskBase(CharacterTreeBase characterBT) : base()
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
