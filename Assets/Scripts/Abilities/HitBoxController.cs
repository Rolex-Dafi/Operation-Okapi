using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    private AbilitySO data;

    public void Init(AbilitySO data)
    {
        this.data = data;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(transform, collision, data, tag);
    }

    public static void HandleCollision(Transform transform, Collider2D collision, AbilitySO attackData, string friendlyTag)
    {
        // check for friendly fire
        if (!collision.CompareTag(friendlyTag))
        {
            // if object with which we collided is damageable, damage it
            if (collision.gameObject.TryGetComponent<IDamagable>(out var damageable))
            {
                damageable.TakeDamage(attackData.damage);
            }

            // if attack has pushback
            if (attackData.enemyPushbackDistance > 0)
            {
                // and object with which we collided is pushable, push/pull it
                if (collision.gameObject.TryGetComponent<IPushable>(out var pushable))
                {
                    pushable.Push(
                        collision.transform.position - transform.position, 
                        attackData.enemyPushbackDistance, 
                        attackData.enemyPushbackSpeed
                        );
                }
            }
        }

    }
}
