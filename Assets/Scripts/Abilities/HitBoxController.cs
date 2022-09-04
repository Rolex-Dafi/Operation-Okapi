using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    private AttackSO data;

    public void Init(AttackSO data)
    {
        this.data = data;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(transform, collision, data, tag);
    }

    public static void HandleCollision(Transform transform, Collider2D collision, AttackSO attackData, string friendlyTag)
    {
        // check for friendly fire
        if (!collision.CompareTag(friendlyTag))
        {
            // if object with which we collided is damageable, damage it
            if (collision.gameObject.TryGetComponent<IDamagable>(out var damageable))
            {
                damageable.TakeDamage(attackData.damage);
            }

            // if attack has pushaback
            if (attackData.enemyPushbackDistance > 0)
            {
                // and object with which we collided is pushable, push it away from this
                if (collision.gameObject.TryGetComponent<IPushable>(out var pushable))
                {
                    Vector2 direction = collision.transform.position - transform.position;
                    pushable.Push(direction, attackData.enemyPushbackDistance, attackData.enemyPushbackSpeed);
                }

            }
        }

    }
}
