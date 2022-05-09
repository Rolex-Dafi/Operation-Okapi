using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles player input which pertains to the controlling of the player character.
/// </summary>
public class PlayerInput : MonoBehaviour
{
    [HideInInspector] public Vector2 move;

    [HideInInspector] public UnityEvent dashButtonDown;

    [HideInInspector] public UnityEvent meleeButtonDown;
    [HideInInspector] public UnityEvent meleeButtonUp;
    [HideInInspector] public UnityEvent rangedButtonDown;
    [HideInInspector] public UnityEvent rangedButtonUp;
    [HideInInspector] public UnityEvent specialButtonDown;

    [HideInInspector] public UnityEvent interactButtonDown;

    public void Init()
    {
        dashButtonDown = new UnityEvent();

        meleeButtonDown = new UnityEvent();
        meleeButtonUp = new UnityEvent();

        rangedButtonDown = new UnityEvent();
        rangedButtonUp = new UnityEvent();

        specialButtonDown = new UnityEvent();

        interactButtonDown = new UnityEvent();
    }

    private void Update()
    {
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Dash"))
        {
            dashButtonDown.Invoke();
        }

        if (Input.GetButtonDown("Melee"))
        {
            meleeButtonDown.Invoke();
        }
        else if (Input.GetButtonUp("Melee"))
        {
            meleeButtonUp.Invoke();
        }

        if (Input.GetButtonDown("Ranged"))
        {
            rangedButtonDown.Invoke();
        }
        else if (Input.GetButtonUp("Ranged"))
        {
            rangedButtonUp.Invoke();
        }

        if (Input.GetButtonDown("Special"))
        {
            specialButtonDown.Invoke();
        }

        if (Input.GetButtonDown("Interact"))
        {
            interactButtonDown.Invoke();
        }
    }
}
