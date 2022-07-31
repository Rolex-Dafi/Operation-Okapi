using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Performs the given attack in the direction of the target set in shared data.
/// </summary>
public class AttackTarget : TaskBase
{
    private string targetName;
    private Vector3? target;
    private Attack attack;

    public AttackTarget(CharacterTreeBase characterBT, Attack attack, string targetName, string debugName = "") : base(characterBT, debugName)
    {
        this.targetName = targetName;
        this.attack = attack;
    }

    protected override void OnBegin()
    {
        // get the taget from shared data
        target = bt.GetItem(targetName) as Vector3?;
        // if it's not in shared data -> report failure
        if (target == null)
        {
            OnEnd(false);
            return;
        }        

        // direct the character towards the target
        bt.Character.Rotate((target.GetValueOrDefault() - bt.Character.transform.position).normalized);

        // try to attack
        if (!bt.Character.Attack(attack))
        {
            OnEnd(false);
            return;
        }
    }

    protected override void OnContinue()
    {
        // ensure we're not playing move anim while attacking
        bt.Character.ForceUpdateSpeed(Vector2.zero);

        // call onEnd with success after attack finishes
        if (!attack.InUse)
        {
            OnEnd(true);
        }

    }
}
