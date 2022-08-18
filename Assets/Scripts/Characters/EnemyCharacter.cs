using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : CombatCharacter
{
    [SerializeField] private DroppedItem droppedItemPrefab;

    public override void Die()
    {
        // drop item
        // so far only money:
        DroppedItem instance = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
        instance.Init(money.GetCurrent());

        // TODO add item drops here + acc. for drop chances

        base.Die();
    }
}
