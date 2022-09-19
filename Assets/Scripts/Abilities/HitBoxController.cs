using UnityEngine;

/// <summary>
/// Controls the hitbox for all melee attacks.
/// </summary>
public class HitBoxController : MonoBehaviour
{
    private AbilitySO data;

    /// <summary>
    /// Initializes the hitbox
    /// </summary>
    /// <param name="data"></param>
    public void Init(AbilitySO data)
    {
        this.data = data;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(transform, collision, data, tag);
    }

    /// <summary>
    /// Handles all player versus enemy collision.
    /// </summary>
    /// <param name="transform">The transform of the object whose collision we're handling</param>
    /// <param name="collision">The collider of the object which collided with the transform</param>
    /// <param name="abilityData">The data </param>
    /// <param name="friendlyTag"></param>
    public static void HandleCollision(Transform transform, Collider2D collision, AbilitySO abilityData, string friendlyTag)
    {
        // check for friendly fire
        if (!collision.CompareTag(friendlyTag))
        {
            // if object with which we collided is damageable, damage it
            if (collision.gameObject.TryGetComponent<IDamagable>(out var damageable))
            {
                damageable.TakeDamage(abilityData.damage);
            }

            // if attack has pushback
            if (abilityData.enemyPushbackDistance > 0)
            {
                // and object with which we collided is pushable, push/pull it
                if (collision.gameObject.TryGetComponent<IPushable>(out var pushable))
                {
                    pushable.Push(
                        collision.transform.position - transform.position, 
                        abilityData.enemyPushbackDistance, 
                        abilityData.enemyPushbackSpeed
                        );
                }
            }
        }

    }
}
