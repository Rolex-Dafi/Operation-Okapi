using UnityEngine;

/// <summary>
/// Triggers the trap this is a child of when the player enters/stands in this collider.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TrapTrigger : MonoBehaviour
{
    [SerializeField] private TrapController trapController;

    private float lastTriggered;
    private const float RepeatTriggerTime = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        lastTriggered = Time.time;
        //TriggerTrap(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // trigger repeatedly if the collider stays in the trap
        if (Time.time - lastTriggered > RepeatTriggerTime) TriggerTrap(other);
    }

    private void TriggerTrap(Collider2D other)
    {
        // ignore if the collider in the trap is not the player
        if (!other.CompareTag(Utility.playerTagAndLayer)) return;
        
        lastTriggered = Time.time;
        trapController.ActivateTrap(other.transform.position);
    }
}
