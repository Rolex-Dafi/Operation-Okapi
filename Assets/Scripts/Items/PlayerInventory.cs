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

    public int LastItemIndex { get; private set; }

    public UnityEvent InventoryChanged; 

    public PlayerInventory(PlayerCharacterSO playerData)
    {
        Equipped = new Item[Utility.maxHealthBarSlots];

        var i = 0;
        foreach (var item in playerData.startingItems)
        {
            Equipped[i] = new Item(item);
            ++i;
        }
        LastItemIndex = i - 1;
        
        InventoryChanged = new UnityEvent();
    }

    /// <summary>
    /// Adds an item to the inventory - this also adds health to the player.
    /// </summary>
    /// <param name="item">Data of the item to add</param>
    public void AddItem(ItemSO item)
    {
        if (!HasSpace()) return;

        // if item of this type already equipped -> don't pick up
        // potential TODO heal the equipped item instead
        if (Equipped.Where(x => x != null).Any(x => item.ID == x.Data.ID)) return;
        
        // add the new item at the beginning of the array
        var next = Equipped[0];
        Equipped[0] = new Item(item);
        for (var i = 1; i <= LastItemIndex; i++)
        {
            (Equipped[i], next) = (next, Equipped[i]); // swaps Equipped[i] and next
        }
        ++LastItemIndex;
        Equipped[LastItemIndex] = next;
        
        InventoryChanged.Invoke();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage">the amount of damage to receive</param>
    /// <returns>the remaining health of the player (i.e sum of hp of all items in inventory)</returns>
    public int ReceiveDamage(int damage)
    {
        // get top item
        var topItem = Equipped[LastItemIndex];
        // only the top item receives damage -> no overkill dmg
        topItem.ReceiveDamage(damage);

        // remove from inventory if destroyed
        if (topItem.CurrentHealth <= 0)
        {
            Equipped[LastItemIndex] = null;
            --LastItemIndex;
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
        return LastItemIndex < Equipped.Length - 1;
    }
    
    public bool ItemEquipped(int id)
    {
        return Equipped.Where(item => item != null).Any(item => item.Data.ID == id);
    }

}
