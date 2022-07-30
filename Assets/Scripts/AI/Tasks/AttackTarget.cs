using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTarget : TaskBase
{
    private Transform target;
    private Attack attack;

    private float timeOfAttack;

    public AttackTarget(CharacterTreeBase characterBT, Transform target, Attack attack) : base(characterBT)
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
        if (Time.time - timeOfAttack > attack.Data.reloadTime) OnEnd(true);
    }
}
