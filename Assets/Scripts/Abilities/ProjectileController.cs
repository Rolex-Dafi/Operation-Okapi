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
        
        Debug.Log("force direction: " + force);
        
        // rotate in the direction of travel
        var rotation = Quaternion.FromToRotation(transform.up, force);
        
        // make it follow isometry rules
        rotation = Quaternion.Euler(
            Utility.isometricAngle,
            rotation.eulerAngles.y,
            rotation.eulerAngles.z
        );
        transform.rotation = rotation;
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
