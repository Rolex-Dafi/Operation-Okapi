using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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

    public void AddItem(ItemSO item)
    {
        // if health bars full -> can't pick up more items
        if (LastItemIndex >= Equipped.Length - 1) return;

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

    // returns the remaining health of the player (i.e sum of hp of all items in inventory)
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

    public bool ItemEquipped(int id)
    {
        return Equipped.Where(item => item != null).Any(item => item.Data.ID == id);
    }

}
