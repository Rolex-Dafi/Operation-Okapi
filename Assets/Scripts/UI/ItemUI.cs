using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image backgroundBox;
    [SerializeField] private Image border;
    [SerializeField] private Image cracked;
    [SerializeField] private Image itemIcon;

    private GameManager gameManager;
    private int lastHealth;

    private bool boxEmpty;
    
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        
        // box to grey, all else alpha = 0
        backgroundBox.color = gameManager.ColorPalette.healthGrey;
        itemIcon.color = new Color(1,1,1,0);
        border.color = new Color(1,1,1,0);
        cracked.color = new Color(1,1,1,0);
        
        boxEmpty = true;
    }
    
    private void AddItem(ItemSO item)
    {
        lastHealth = item.Health;
        itemIcon.sprite = item.UISprite;
        
        // tween box color to red, border + icon to full alpha
        backgroundBox.DOColor(gameManager.ColorPalette.healthRed, .3f);
        itemIcon.DOColor(Color.white, .1f);
        border.DOColor(Color.white, .3f);
        // punch scale
        transform.DOPunchScale(Vector3.one * 0.5f, .4f, 1, 0);
        
        boxEmpty = false;
    }
    
    public void UpdateItem(Item item)
    {
        if (item == null)
        {
            // if incoming null but item is set -> destroy it
            if (!boxEmpty) DestroyItem(); 
            
            return;
        }
        
        // if item not present - add it
        if (boxEmpty)
        {
            AddItem(item.Data);
        }
        
        // not damaged this time
        if (lastHealth == item.CurrentHealth) return;
        
        // tween
        backgroundBox.PunchColor(gameManager.ColorPalette.healthBrightRed, .3f);
        backgroundBox.PunchAlpha(1, .3f);
        transform.DOPunchScale(-Vector3.one * 0.1f, .3f, 1, 0);
        
        if (item.CurrentHealth == 1) CrackItem();
        else if (item.CurrentHealth <= 0) DestroyItem();
        
        lastHealth = item.CurrentHealth;
    }
    
    private void CrackItem()
    {
        // reduce alpha of item icon
        itemIcon.DOColor(new Color(1,1,1,.5f), .15f); 
        
        // crack to full alpha
        cracked.DOColor(Color.white, .15f);
    }
    
    private void DestroyItem()
    {
        // box color to grey, all else to transparent
        backgroundBox.DOColor(gameManager.ColorPalette.healthGrey, .3f);
        itemIcon.DOColor(new Color(1, 1, 1, 0), .1f);
        border.DOColor(new Color(1, 1, 1, 0), .3f);
        cracked.DOColor(new Color(1, 1, 1, 0), .3f);
        // punch scale
        transform.DOPunchScale(-Vector3.one * 0.3f, .3f, 1, 0);
        
        boxEmpty = true;
    }
}
