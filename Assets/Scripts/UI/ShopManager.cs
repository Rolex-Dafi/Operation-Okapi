using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    
    [SerializeField] private ShopItemUI[] shopItemTemplates;

    private GameManager gameManager;
    private MerchantCharacter merchant;
    
    public void Init(GameManager gameManager, MerchantCharacter merchant)
    {
        this.gameManager = gameManager;
        this.merchant = merchant;
        canvasGroup.alpha = 0;
    }

    /// <summary>
    /// Expects an array of 3 items.
    /// </summary>
    /// <param name="items"></param>
    /// <param name="playerMoneyAmount"></param>
    public void ShowShop(ItemSO[] items, int playerMoneyAmount)
    {
        for (int i = 0; i < shopItemTemplates.Length; i++)
        {
            // if part of shop bought out
            if (i >= items.Length)
            {
                shopItemTemplates[i].Hide();
            }
            else
            {
                var canAfford = playerMoneyAmount >= items[i].Cost;
                shopItemTemplates[i].Init(items[i], canAfford, BuyItem);
            }
        }
        
        // TODO tween
        canvasGroup.alpha = 1;
        gameManager.PauseGame(true);
    }

    private void BuyItem(ItemSO item)
    {
        // let the merchant know
        merchant.Sell(item);

        // close the shop
        Close();
    }
    
    public void Close()
    {
        // TODO tween
        canvasGroup.alpha = 0;
        gameManager.PauseGame(false);
    }
}
