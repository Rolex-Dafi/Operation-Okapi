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

    //private float timeOfAttack;

    public AttackTarget(CharacterTreeBase characterBT, Attack attack, string targetName) : base(characterBT)
    {
        this.targetName = targetName;
        this.attack = attack;
    }

    protected override void OnBegin()
    {
        // get the taget from shared data
        if (target == null)
        {
            target = bt.GetItem(targetName) as Vector3?;
            // if it's not in shared data -> report failure
            if (target == null)
            {
                OnEnd(false);
            }
        }

        // direct the character towards the target
        bt.Character.Rotate(((Vector3)target - bt.Character.transform.position).normalized);

        // try to attack
        if (!bt.Character.Attack(attack))
        {
            OnEnd(false);
            return;
        }

        // TODO wait for attack to finish - might not be neccessary if this is used correctly though
        // (i.e. a wait task is called after this, for the attack's recovery duration set in attack data)
        OnEnd(true);
    }

    protected override void OnContinue()
    {

    }
}
