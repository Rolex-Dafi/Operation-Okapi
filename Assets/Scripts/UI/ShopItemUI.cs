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
    
    public void Init(ItemSO item, bool canInteract, UnityAction<ItemSO> onBuyPressed)
    {
        itemIcon.sprite = item.UISprite;
        nameText.text = item.ItemName;
        priceText.text = item.Cost + " coins";

        buyButton.interactable = canInteract;
        buyButton.onClick.AddListener(() => onBuyPressed?.Invoke(item));
        
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
