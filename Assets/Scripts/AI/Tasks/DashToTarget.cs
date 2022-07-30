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

    // for targets known at the time this character's bt is initialized
    public DashToTarget(CharacterTreeBase characterBT, Vector3 target) : base(characterBT)
    {
        this.target = target;
    }

    // for targets set at runtime by other tasks in this character's bt
    public DashToTarget(CharacterTreeBase characterBT, string targetName) : base(characterBT)
    {
        this.targetName = targetName;
    }

    protected override void OnBegin()
    {
        // if target not set at the beginning, try to get it from shared data
        if (target == null)
        {
            target = bt.GetItem(targetName) as Vector3?;
            // if it's not in shared data either -> report failure
            if (target == null)
            {
                OnEnd(false);
            }
        }

        // rotate char tw target
        bt.Character.Rotate(((Vector3)target - bt.Character.transform.position).normalized);

        // call dash
        bt.Character.Dash();
    }

    protected override void OnContinue()
    {
        // call onEnd with success after dash finishes
        if (!bt.Character.GetDash().CurrentlyDashing)
        {
            OnEnd(true);
        }
    }
}
