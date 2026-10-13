using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Base class for the merchant.
/// </summary>
[RequireComponent(typeof(Interactable))]
public class MerchantCharacter : Character
{
    [SerializeField] private bool canShop = true;
    
    [SerializeField] private Level level = Level.Office;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private Interactable interactable;
    
    private List<ItemSO> currentShop;
    
    private GameManager gameManager;

    private void Awake()
    {
        Init();
    }

    public override void Init()
    {
        currentShop = (data as MerchantCharacterSO)?.shop.ToList();
        base.Init();
        
        // set the correct animation according to level
        Animator.SetTrigger(level.ToString());

        gameManager = FindObjectOfType<GameManager>();
        shopManager.Init(gameManager, this);

        if (!canShop) return;
        
        interactable.Init("Press " + EButtonDown.Interact.GetButtonName() + " to shop");
        interactable.onInteractPressed.AddListener(OpenShop);
    }

    private void OpenShop()
    {
        if (!canShop) return;
        
        // select three items from the current shop
        var itemsToShow = new ItemSO[3];
        var i = 0;
        foreach (var itemSo in currentShop)
        {
            // only show items which the player doesn't currently have
            if (!gameManager.PlayerCharacterInstance.Inventory.ItemEquipped(itemSo.ID))
            {
                itemsToShow[i] = itemSo;
                ++i;
            }

            if (i >= 3) break; // stop if the shop is filled
        }
        
        // show the items
        shopManager.ShowShop(itemsToShow);
    }

    /// <summary>
    /// Sell given item - should be called from shop manager, shop manager is responsible
    /// for checking if the player has enough money (and sets the buttons to non-interactable if not).
    /// This trades money (according to the item's cost) from player to merchant and puts
    /// the item in the player's inventory.
    /// </summary>
    /// <param name="item">The item data for the item we're selling</param>
    public void Sell(ItemSO item)
    {
        // remove item from shop
        currentShop.Remove(item);
        
        // add money and remove it from player
        gameManager.PlayerCharacterInstance.Money.ChangeCurrent(-item.Cost);
        money.ChangeCurrent(item.Cost);
        
        // give the item to the player
        gameManager.PlayerCharacterInstance.CollectItem(item);
    }

}
