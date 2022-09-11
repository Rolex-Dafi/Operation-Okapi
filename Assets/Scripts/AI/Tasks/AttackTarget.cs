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
    private bool precise;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="characterBT"></param>
    /// <param name="attack"></param>
    /// <param name="targetName"></param>
    /// <param name="precise">Should we attack the target specifically or attack in the direction of the target</param>
    /// <param name="debugName"></param>
    public AttackTarget(CharacterTreeBase characterBT, Attack attack, string targetName, bool precise = false, string debugName = "") : base(characterBT, debugName)
    {
        this.targetName = targetName;
        this.attack = attack;
        this.precise = precise;
    }

    protected override void OnBegin()
    {
        // get the target from shared data
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
        if (precise)
        {
            if (!bt.Character.AttackTarget(attack, target.GetValueOrDefault()))
            {
                OnEnd(false);
            }
        }
        else
        {
            if (!bt.Character.Attack(attack))
            {
                OnEnd(false);
            }
        }
        
    }

    protected override void OnContinue()
    {
        // ensure we're not playing move anim while attacking
        bt.Character.ForceIdle();

        // call onEnd with success after attack finishes
        if (!attack.InUse)
        {
            OnEnd(true);
        }

    }
}
