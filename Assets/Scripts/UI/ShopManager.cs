using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMPro.TextMeshPro nameText;
    [SerializeField] private TMPro.TextMeshPro priceText;
    
    public void Init(ItemSO item)
    {
        itemIcon.sprite = item.UISprite;
        nameText.text = item.ItemName;
        priceText.text = item.Cost + " coins";
    }
}
