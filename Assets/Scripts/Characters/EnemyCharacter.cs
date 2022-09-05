using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyCharacter : CombatCharacter
{
    [SerializeField] private DroppedItem droppedItemPrefab;

    [SerializeField] private ItemSO[] drops;

    private PlayerCharacter playerCharacter;

    private CharacterTreeBase enemyAI;

    public void Freeze(bool freeze)
    {
        enemyAI.ShouldUpdate = !freeze;
    }
    
    public void Init(PlayerCharacter playerCharacter)
    {
        this.playerCharacter = playerCharacter;
        base.Init();

        enemyAI = GetComponent<CharacterTreeBase>(); // ! this assumes no subtrees !
        enemyAI.ShouldUpdate = true;
    }
    
    public override void Die()
    {
        // drop item
        // TODO bug - instantiate item under room so it gets destroyed when exiting the room
        DroppedItem instance = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
        //instance.Init(money.GetCurrent());

        // TODO add item drops here + acc. for drop chances
        // for testing - 100% drop chance
        // TODO don't drop items which are already spawned (dropped by previous enemies)
        instance.Init(GetDrop());

        base.Die();
    }

    private ItemSO GetDrop()
    {
        // return a random drop which isn't already in the player inventory
        var viableDrops = drops.
            Where(drop => !playerCharacter.Inventory.ItemEquipped(drop.ID)).
            ToList();

        return viableDrops.Count > 0 ? 
            viableDrops[Random.Range(0, viableDrops.Count - 1)] : 
            drops[Random.Range(0, drops.Length - 1)];
    }
}
