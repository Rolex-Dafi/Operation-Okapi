using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackTarget : TaskBase
{
    private Transform target;
    private float reloadTime;

    private float timeOfAttack;

    public RangedAttackTarget(CharacterTreeBase characterBT, Transform target, float reloadTime) : base(characterBT)
    {
        this.target = target;
        this.reloadTime = reloadTime;
    }

    protected override void OnBegin()
    {
        // direct the character towards the target
        bt.Character.Rotate((target.position - bt.Character.transform.position).normalized);

        // attack
        if (!bt.Character.RangedAttack())
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

        // call on end after reload finished time
        if (Time.time - timeOfAttack > reloadTime) OnEnd(true);
    }
}
