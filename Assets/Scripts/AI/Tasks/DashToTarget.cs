using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Performs a dash in the direction of the target.
/// </summary>
public class DashToTarget : TaskBase
{
    private Vector3? target;
    private string targetName;

    // for targets set at runtime by other tasks in this character's bt
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
