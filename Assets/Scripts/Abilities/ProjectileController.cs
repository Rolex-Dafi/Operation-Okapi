using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileController : MonoBehaviour
{
    private AttackSO data;

    private Rigidbody2D rb;

    private string friendlyTag;

    public void Init(AttackSO data, string friendlyTag)
    {
        this.data = data;

        this.friendlyTag = friendlyTag;
        gameObject.tag = friendlyTag;

        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(Vector2 force)
    {
        StartCoroutine(rb.AddForceCustom(force, data.attackRange, data.projectileSpeed, OnEnd));
        
        // rotate in the direction of travel
        transform.rotation = Quaternion.FromToRotation(transform.up, force);

        // make it follow isometry rules
        Vector3 eulerAngles = Quaternion.FromToRotation(transform.up, force).eulerAngles;
        transform.rotation = Quaternion.Euler(
            Utility.isometricAngle,
            eulerAngles.y,
            eulerAngles.z
        );
    }

    // TODO add animation/particle effect
    private void OnEnd()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HitBoxController.HandleCollision(transform, collision, data, friendlyTag);
    }
}
