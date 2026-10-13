using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Class for all enemy characters.
/// </summary>
public class EnemyCharacter : CombatCharacter
{
    [SerializeField] private DroppedItem droppedItemPrefab;

    [SerializeField] private ItemSO[] drops;
    
    private PlayerCharacter playerCharacter;

    private CharacterTreeBase enemyAI;

    /// <summary>
    /// Stops the AI from updating (useful ex. when pausing the game).
    /// </summary>
    /// <param name="freeze"></param>
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
        // drop item - instantiate it under the current room so it gets destroyed when exiting the room
        var instance = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity, LevelManager.CurrentRoomTransform);

        if (drops.Length > 0)
        {
            // flip a coin -> drop either money or an item
            if (Random.Range(0, 2) == 0)
            {
                instance.Init(money.GetCurrent());
            }
            else
            {
                var playerChar = FindObjectOfType<PlayerCharacter>();
                if (playerChar != null && !playerChar.Inventory.HasSpace()) // only drop money if inventory full
                {
                    instance.Init(money.GetCurrent());
                }
                else
                {
                    // TODO don't drop items which are already spawned (dropped by previous enemies)
                    instance.Init(GetDrop());
                }
            }
        }

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
