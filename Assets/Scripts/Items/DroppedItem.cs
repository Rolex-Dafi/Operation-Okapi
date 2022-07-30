using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class DroppedItem : MonoBehaviour
{
    // so far only money
    private int amount;
    [SerializeField] private EventReference onCollectSound;

    public void Init(int amount)
    {
        this.amount = amount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Utility.playerTagAndLayer))
        {
            // check if we're colliding with the player character specifically (not the hit box or a projectile)
            if (collision.TryGetComponent<PlayerCharacter>(out var character))
            {
                character.Collect(amount);
                RuntimeManager.PlayOneShot(onCollectSound.Guid);
                Destroy(gameObject);
            }
        }
    }

}
