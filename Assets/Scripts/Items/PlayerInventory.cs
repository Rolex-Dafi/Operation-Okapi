using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory
{
    // static length - only as big as the max health slots
    public Item[] Equipped { get; private set; }

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
        // TODO if the last item is damaged - add this as second last - i.e. before the damaged item

        ++LastItemIndex;
        Equipped[LastItemIndex] = new Item(item);
        
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

}
