using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Player inventory wrapper class. The player inventory equals the items the player has
/// currently equipped.
/// </summary>
public class PlayerInventory
{
    // static length - only as big as the max health slots
    public Item[] Equipped { get; }

    private int lastItemIndex;

    public UnityEvent InventoryChanged; 

    public PlayerInventory(PlayerCharacterSO playerData)
    {
        Equipped = new Item[Utility.maxHealthBarSlots];
        InventoryChanged = new UnityEvent();
        lastItemIndex = -1;
        
        foreach (var item in playerData.startingItems)
        {
            AddItem(item);
        }
        
    }

    /// <summary>
    /// Adds an item to the inventory - this also adds health to the player.
    /// </summary>
    /// <param name="item">Data of the item to add</param>
    public void AddItem(ItemSO item)
    {
        // if item of this type already equipped -> don't pick up
        // potential TODO heal the equipped item instead
        if (Equipped.Where(x => x != null).Any(x => item.ID == x.Data.ID)) return;
        
        // add the new item at the beginning of the array
        var next = Equipped[0];
        Equipped[0] = new Item(item);
        for (var i = 1; i <= lastItemIndex; i++)
        {
            (Equipped[i], next) = (next, Equipped[i]); // swaps Equipped[i] and next
        }
        ++lastItemIndex;
        if (next != null) Equipped[lastItemIndex] = next;   // if next == null, this is the first item
        
        InventoryChanged.Invoke();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage">the amount of damage to receive</param>
    /// <returns>the remaining health of the player (i.e sum of hp of all items in inventory)</returns>
    public int ReceiveDamage(int damage)
    {
        // in case the player is dead
        if (lastItemIndex < 0) return 0;

        // get top item
        var topItem = Equipped[lastItemIndex];
        // only the top item receives damage -> no overkill dmg
        topItem.ReceiveDamage(damage);

        // remove from inventory if destroyed
        if (topItem.CurrentHealth <= 0)
        {
            Equipped[lastItemIndex] = null;
            --lastItemIndex;
        }
        
        InventoryChanged.Invoke();

        // return the current health of the player = sum of current hp of all equipped items
        return Equipped.Where(item => item != null).Sum(item => item.CurrentHealth);
    }

    /// <summary>
    /// Is there space in the inventory for another item?
    /// </summary>
    /// <returns>If there is space</returns>
    public bool HasSpace()
    {
        // if health bars full -> can't pick up more items
        return lastItemIndex < Equipped.Length - 2;
    }
    
    /// <summary>
    /// Checks whether an item is currently equipped.
    /// </summary>
    /// <param name="id">The id of the item to check</param>
    /// <returns></returns>
    public bool ItemEquipped(int id)
    {
        return Equipped.Where(item => item != null).Any(item => item.Data.ID == id);
    }

}
