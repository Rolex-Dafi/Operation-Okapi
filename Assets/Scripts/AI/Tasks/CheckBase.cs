using BehaviourTree;

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
        // ! assumes check is fast enough to be performed in a single frame !
        status = Check() ? NodeStatus.Success : NodeStatus.Failure;

        return status;
    }

    protected abstract bool Check();
}
