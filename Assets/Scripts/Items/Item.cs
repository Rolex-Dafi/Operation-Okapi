using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    private ItemSO data;
    private Health health;

    public Item(ItemSO data, Health health)
    {
        this.data = data;
        this.health = health;
    }

    public void ReceiveDamage(int damage)
    {

    }
}
