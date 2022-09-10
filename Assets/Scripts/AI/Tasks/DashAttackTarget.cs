using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAttackTarget : TaskBase
{
    private string targetName;
    private Vector3? target;
    private Attack attack;

    private bool alreadyAttacked;

    public DashAttackTarget(CharacterTreeBase characterBT, Attack attack, string targetName, string debugName = "") : base(characterBT, debugName)
    {
        this.targetName = targetName;
        this.attack = attack;
    }

    protected override void OnBegin()
    {
        // reset
        alreadyAttacked = false;

        // get the taget from shared data
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
        // try attacking after dash finishes
        if (!bt.Character.GetDash().InUse)
        {
            // try to attack if hasn't attacked yet
            if (!alreadyAttacked)
            {
                // rotate char tw target
                bt.Character.Rotate(((Vector3)target - bt.Character.transform.position).normalized);

                alreadyAttacked = true;
                if (!bt.Character.Attack(attack))
                {
                    OnEnd(false);
                    return;
                }
            }

            // ensure we're not playing move anim while attacking
            bt.Character.ForceIdle();

            // call onEnd with success after attack finishes
            if (!attack.InUse)
            {
                OnEnd(true);
            }
        }

    }
}
