using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
public class DroppedItem : MonoBehaviour
{
    // always one or the other
    private int money; // TODO probably best if I make money into ItemSO ? bc of sprites
    private ItemSO item;

    [SerializeField] private EventReference onCollectSound;

    public void Init(int amount)
    {
        money = amount;
        item = null;
        // TODO change money sprite according to amount dropped
    }

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
                if (money > 0) character.CollectMoney(money);
                else if (item != null) character.CollectItem(item);

                RuntimeManager.PlayOneShot(onCollectSound.Guid);
                Destroy(gameObject);
            }
        }
    }

}
