using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles player input which pertains to the controlling of the player character.
/// </summary>
public class PlayerInput : MonoBehaviour
{
    [HideInInspector] public Vector2 move;

    [HideInInspector] public UnityEvent dashEvent;

    [HideInInspector] public UnityEvent meleeAttackEvent;
    [HideInInspector] public UnityEvent rangedAttackEvent;
    [HideInInspector] public UnityEvent specialAttackEvent;

    [HideInInspector] public UnityEvent interactEvent;

    public void Init()
    {
        dashEvent = new UnityEvent();
        meleeAttackEvent = new UnityEvent();
        rangedAttackEvent = new UnityEvent();
        specialAttackEvent = new UnityEvent();

        interactEvent = new UnityEvent();
    }

    private void Update()
    {
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Dash"))
        {
            dashEvent.Invoke();
        }

        if (Input.GetButtonDown("Melee"))
        {
            meleeAttackEvent.Invoke();
        }

        if (Input.GetButtonDown("Ranged"))
        {
            rangedAttackEvent.Invoke();
        }

        if (Input.GetButtonDown("Special"))
        {
            specialAttackEvent.Invoke();
        }

        if (Input.GetButtonDown("Interact"))
        {
            interactEvent.Invoke();
        }
    }
}
