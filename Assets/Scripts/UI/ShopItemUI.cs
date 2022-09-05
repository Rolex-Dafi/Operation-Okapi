using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        nameText.text = item.ItemName;
        priceText.text = item.Cost + " coins";

        buyButton.interactable = canInteract;
        buyButton.onClick.AddListener(() =>
        {
            onBuyPressed?.Invoke(item); // buy the item
            Hide();                     // empty the panel
        });

        currentItem = item;
        
        gameObject.SetActive(true);
    }

    public void UpdateItem(PlayerCharacter playerCharacter)
    {
        // player has enough money and has space in the inventory
        buyButton.interactable = playerCharacter.Money.GetCurrent() >= currentItem.Cost && playerCharacter.Inventory.HasSpace();
    }
    
    public void Hide()
    {
        buyButton.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }
}
