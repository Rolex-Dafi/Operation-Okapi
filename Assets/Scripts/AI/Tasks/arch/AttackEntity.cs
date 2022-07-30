using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Performs the given attack in the direction of the entity.
/// </summary>
public class AttackEntity : TaskBase
{
    private Transform target;
    private Attack attack;

    private float timeOfAttack;

    public AttackEntity(CharacterTreeBase characterBT, Transform target, Attack attack) : base(characterBT)
    {
        this.target = target;
        this.attack = attack;
    }

    protected override void OnBegin()
    {
        // direct the character towards the target
        bt.Character.Rotate((target.position - bt.Character.transform.position).normalized);

        // attack
        if (!bt.Character.Attack(attack))
        {
            OnEnd(false);
            return;
        }
        timeOfAttack = Time.time;
    }

    protected override void OnContinue()
    {
        // reload -> make sure the character stays in place
        bt.Character.Move(Vector2.zero);

        // call on end after reload finished
        if (Time.time - timeOfAttack > attack.Data.recoveryTime) OnEnd(true);
    }
}
