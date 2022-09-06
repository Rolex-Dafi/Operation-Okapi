using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickRangedAttack : RangedAttack
{
    public ClickRangedAttack(CombatCharacter character, AttackSO data) : base(character, data) { }

    public override void OnBegin()
    {
        base.OnBegin();
        SpawnProjectile(); // TODO invoke after delay specified in data
        //character.Invoke("SpawnProjectile", (data as AttackSO).projectileDelay);
    }

}
