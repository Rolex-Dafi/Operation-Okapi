using UnityEngine;

/// <summary>
/// Health bar UI manager class.
/// </summary>
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
        // update item gfx (ex. if any item is cracked or destroyed) or if they changed positions
        for (var i = 0; i < items.Length; i++)
        {
            items[i].UpdateItem(playerInventory.Equipped[i]);
        }
    }
}
