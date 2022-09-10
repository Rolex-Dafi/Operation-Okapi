using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles items dropped by the enemy characters and their collection by the player.
/// </summary>
[RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
public class DroppedItem : MonoBehaviour
{
    // always either one or the other
    private int money; 
    private ItemSO item;

    [SerializeField] private EventReference onCollectSound;

    /// <summary>
    /// Initializes the dropped item with money.
    /// </summary>
    /// <param name="amount">The amount of money to drop</param>
    public void Init(int amount)
    {
        money = amount;
        item = null;
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Utility.playerTagAndLayer))
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
        }
    }

}
