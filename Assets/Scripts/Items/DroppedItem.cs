using System;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles items dropped by the enemy characters and their collection by the player.
/// </summary>
[RequireComponent(typeof(Interactable), typeof(SpriteRenderer))]
public class DroppedItem : MonoBehaviour
{
    [SerializeField] private bool initInAwake;
    
    // always either one or the other
    private int money; 
    [SerializeField] private ItemSO item;

    [SerializeField] private Interactable interactable;
    
    [SerializeField] private EventReference onCollectSound;

    private void Awake()
    {
        if (initInAwake)
        {
            Init();
        }
    }

    /// <summary>
    /// Initializes the dropped item with money.
    /// </summary>
    /// <param name="amount">The amount of money to drop</param>
    public void Init(int amount)
    {
        money = amount;
        item = null;

        Init();
    }

    /// <summary>
    /// Initializes the dropped item with an item.
    /// </summary>
    /// <param name="item">The data of the item to drop</param>
    public void Init(ItemSO item)
    {
        this.item = item;
        money = 0;
        GetComponent<SpriteRenderer>().sprite = item.WorldSprite;  // the prefabs base sprite is money

        Init();
    }

    private void Init()
    {
        interactable = GetComponent<Interactable>();
        interactable.Init("Press " + EButtonDown.Interact.GetButtonName() + " to pick up");
        interactable.onInteractPressed.AddListener(CollecItem);
    }

    private void CollecItem()
    {
        var character = interactable.PlayerCharacter;
        if (character == null) return;
        
        if (money > 0)
        {
            character.CollectMoney(money);
            RuntimeManager.PlayOneShot(onCollectSound.Guid);
            Destroy(gameObject);
        }
        else if (item != null)
        {
            // only destroy the item if item added successfully
            character.CollectItem(item, () =>
            {
                RuntimeManager.PlayOneShot(onCollectSound.Guid);
                Destroy(gameObject);
            });
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*if (collision.CompareTag(Utility.playerTagAndLayer))
        {
            // check if we're colliding with the player character specifically (not the hit box or a projectile)
            if (collision.TryGetComponent<PlayerCharacter>(out var character))
            {
                if (money > 0)
                {
                    character.CollectMoney(money);
                    RuntimeManager.PlayOneShot(onCollectSound.Guid);
                    Destroy(gameObject);
                }
                else if (item != null)
                {
                    // only destroy the item if item added successfully
                    character.CollectItem(item, () =>
                    {
                        RuntimeManager.PlayOneShot(onCollectSound.Guid);
                        Destroy(gameObject);
                    });
                }
            }
        }*/
    }

}
