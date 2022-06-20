using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackTarget : TaskBase
{
    private Vector3 target;
    private float reloadTime;

    private float timeOfAttack;

    public RangedAttackTarget(CharacterTreeBase characterBT, Vector3 target, float reloadTime) : base(characterBT)
    {
        this.target = target;
        this.reloadTime = reloadTime;
    }

    protected override void OnBegin()
    {
        // direct the character towards the target
        bt.Character.Rotate((target - bt.Character.transform.position).normalized);

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
        // reload and call on end after some time
        if (Time.time - timeOfAttack > reloadTime) OnEnd(true);
    }
}
