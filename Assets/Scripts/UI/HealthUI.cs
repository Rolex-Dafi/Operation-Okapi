using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private ItemUI itemPrefab;
    private ItemUI[] items;
    
    private PlayerInventory playerInventory;

    public void Init(GameManager gameManager, PlayerInventory playerInventory)
    {
        this.playerInventory = playerInventory;
        
        // listen to inventory changes
        playerInventory.InventoryChanged.AddListener(UpdateUI);
        
        // spawn and init all item boxes
        items = new ItemUI[Utility.maxHealthBarSlots];
        for (var i = 0; i < items.Length; i++)
        {
            items[i] = Instantiate(itemPrefab, transform);
            items[i].Init(gameManager);
        }
        
        // update the UI acc. to current inventory
        UpdateUI();
    }

    private void UpdateUI()
    {
        // TODO add tweens - ex. punch item size when changing order
        // TODO punch color when cracking or taking dmg etc. <- in ItemUI
        
        // check for new items:
        // TODO update item order in UI
        
        // update item gfx (ex. if any item is cracked or destroyed)
        for (var i = 0; i < items.Length; i++)
        {
            if (playerInventory.Equipped[i] != null) Debug.Log("updating item " + playerInventory.Equipped[i].Data.ItemName);
            items[i].UpdateItem(playerInventory.Equipped[i]);
        }
    }
}
