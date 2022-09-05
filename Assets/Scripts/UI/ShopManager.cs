using UnityEngine;
using DG.Tweening;

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
    /// Opens the shop overlay.
    /// </summary>
    /// <param name="items">The data of the items to sell. (Expects an array of 3 items or less.)</param>
    public void ShowShop(ItemSO[] items)
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
                // player has enough money and has space in the inventory
                var canBuy = gameManager.PlayerCharacterInstance.Money.GetCurrent() >= items[i].Cost && 
                             gameManager.PlayerCharacterInstance.Inventory.HasSpace();
                shopItemTemplates[i].Init(items[i], canBuy, BuyItem);
            }
        }
        
        // TODO better tween - scale etc.
        canvasGroup.DOFade(1, .1f);
        gameManager.PauseGame(true);
    }

    private void BuyItem(ItemSO item)
    {
        // let the merchant know
        merchant.Sell(item);

        // update shop
        foreach (var itemTemplate in shopItemTemplates)
        {
            itemTemplate.UpdateItem(gameManager.PlayerCharacterInstance);
        }
    }
    
    /// <summary>
    /// Closes the shop overlay.
    /// </summary>
    public void Close()
    {
        // TODO better tween
        canvasGroup.DOFade(0, .1f);
        gameManager.PauseGame(false);
    }
}
