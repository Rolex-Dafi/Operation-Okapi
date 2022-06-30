using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickRangedAttack : RangedAttack
{
    public ClickRangedAttack(CombatCharacter character, AttackSO data) : base(character, data) { }

    public override void OnBegin()
    {
        SpawnProjectile();
    }

}
