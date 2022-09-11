using UnityEngine;

/// <summary>
/// Performs a dash in the direction of the target.
/// </summary>
public class DashToTarget : TaskBase
{
    private Vector3? target;
    private string targetName;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="characterBT">The behavioral tree of this character</param>
    /// <param name="targetName">The name of the target - this task will try to retrieve the target from shared memory.</param>
    /// <param name="debugName"></param>
    public DashToTarget(CharacterTreeBase characterBT, string targetName, string debugName = "") : base(characterBT, debugName)
    {
        this.targetName = targetName;
    }

    protected override void OnBegin()
    {
        // try to get it from shared data
        target = bt.GetItem(targetName) as Vector3?;        
        // if it's not in shared data -> report failure
        if (target == null)
        {
            OnEnd(false);
            return;
        }

        // rotate char tw target
        bt.Character.Rotate(((Vector3)target - bt.Character.transform.position).normalized);

        // call dash
        bt.Character.Dash();
    }

    protected override void OnContinue()
    {
        // call onEnd with success after dash finishes
        if (!bt.Character.GetDash().InUse)
        {
            OnEnd(true);
        }
    }
}
