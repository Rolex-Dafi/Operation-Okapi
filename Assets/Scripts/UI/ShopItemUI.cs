using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Manager for the UI of items in the shop.
/// </summary>
public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;

    [SerializeField] private Button buyButton;

    private ItemSO currentItem;
    
    public void Init(ItemSO item, bool canInteract, UnityAction<ItemSO> onBuyPressed)
    {
        itemIcon.sprite = item.UISprite;
        nameText.text = "+" + item.Health + " HP";
        priceText.text = item.Cost + " $";

        buyButton.interactable = canInteract;
        buyButton.onClick.AddListener(() =>
        {
            onBuyPressed?.Invoke(item); // buy the item
            Hide();                     // empty the panel
        });

        currentItem = item;
        
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Updates the item UI according to the current player state.
    /// </summary>
    /// <param name="playerCharacter">The current player character instance</param>
    public void UpdateItem(PlayerCharacter playerCharacter)
    {
        // player has enough money and has space in the inventory
        buyButton.interactable = playerCharacter.Money.GetCurrent() >= currentItem.Cost && playerCharacter.Inventory.HasSpace();
    }
    
    /// <summary>
    /// Hides this item from the shop UI.
    /// </summary>
    public void Hide()
    {
        buyButton.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }
}
