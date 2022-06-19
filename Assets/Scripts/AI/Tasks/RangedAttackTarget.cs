using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackTarget : BTTaskBase
{
    private Vector3 target;

    public RangedAttackTarget(BTTreeBase characterBT, Vector3 target) : base(characterBT)
    {
        this.target = target;
    }

    protected override void OnBegin()
    {
        // attack
    }

    protected override void OnContinue()
    {
        // reload and call on one after some time
    }
}
