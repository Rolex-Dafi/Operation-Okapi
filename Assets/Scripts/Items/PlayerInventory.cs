using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    private Item[] inventory;

    private int lastItemIndex;

    public void Init(PlayerCharacterSO playerData)
    {
        inventory = new Item[Utility.maxHealthBarSlots];

        int i = 0;
        foreach (var item in playerData.startingItems)
        {
            inventory[i] = new Item(item, new Health(item.Health));
            ++i;
        }
        lastItemIndex = i - 1;
    }

    public void AddItem(ItemSO item)
    {

    }

    public void ReceiveDamage(int damage)
    {
        // get last item

        // deduce health
        // no overkill - i.e. if the dmg is more then remaining hp on the last item -> only destroy that item
    }

}
