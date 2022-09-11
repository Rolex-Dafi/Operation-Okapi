using UnityEngine;

/// <summary>
/// Handles the behaviour when the trap is triggered, ex. whether it should deal damage.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TrapDamager : MonoBehaviour
{
    private TrapSO data;
    
    public void Init(TrapSO data)
    {
        this.data = data;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HitBoxController.HandleCollision(transform, collision, data, tag);
    }
}
