using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyCharacter : CombatCharacter
{
    [SerializeField] private DroppedItem droppedItemPrefab;

    [SerializeField] private ItemSO[] drops;
    
    public override void Die()
    {
        // drop item
        // so far only money:
        DroppedItem instance = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
        //instance.Init(money.GetCurrent());

        // TODO add item drops here + acc. for drop chances
        // for testing - 100% drop chance
        // TODO don't drop items which are already spawned (dropped by previous enemies) and those in current player inventory
        instance.Init(drops[Random.Range(0, drops.Length - 1)]);

        base.Die();
    }
}
