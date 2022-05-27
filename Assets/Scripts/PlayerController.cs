using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Connects the player input and player character, issuing commands
/// to the character as a reaction to player input. 
/// </summary>
public class PlayerController : MonoBehaviour
{
    private PlayerCharacter playerCharacter;
    private PlayerInput playerInput;

    private bool aiming;
    private Vector2 targetPosition;

    public bool Aiming { get => aiming; set => aiming = value; }
    public Vector2 TargetPosition { get => targetPosition; private set => targetPosition = value; }

    private void Start()
    {
        // player character
        playerCharacter = GetComponent<PlayerCharacter>();
        playerCharacter.Init();

        // player input
        playerInput = GetComponent<PlayerInput>();
        playerInput.Init();

        // axis events
        //playerInput.moveEvent.AddListener(OnMove);

        // button down events
        foreach (EButtonDown interaction in Enum.GetValues(typeof(EButtonDown)))
        {
            UnityAction<EButtonDown> action;
            if (interaction == EButtonDown.Dash) action = OnDash;
            else if (interaction == EButtonDown.Interact) action = OnInteract;
            else action = OnAttack;

            playerInput.buttonDownEvents[interaction].AddListener(action);
        }

        // button up events
        foreach (EButtonUp interaction in Enum.GetValues(typeof(EButtonUp)))
        {
            playerInput.buttonUpEvents[interaction].AddListener(OnAttack);
        }
    }

    private void FixedUpdate()
    {
        playerCharacter.Move(playerInput.movement.normalized);

        if (aiming) Aim();
    }

    private void Aim()
    {
        Vector2 direction;

        if (playerInput.gamepadConnected)
        {
            // when using gamepad get direction from movement input - already set in playerCharacter
            direction = playerCharacter.Facing;

            // set target inf front of the player in the direction they're facing
            targetPosition = transform.position.ToVector2() + direction;
        }
        else
        {
            // get world mouse position
            targetPosition = Camera.main.ScreenToWorldPoint(playerInput.mousePosition);

            // player faces in the direction of the mouse
            direction = (targetPosition - transform.position.ToVector2());
        }

        // rotate the character
        playerCharacter.Rotate(direction.normalized);

        // rotate the aiming gfx as well
        playerCharacter.RotateAimingGFX();
    }

    private void OnDash<T>(T interaction)
    {
        Debug.Log("player wants to dash!");
        playerCharacter.Dash();
    }

    private void OnAttack<T>(T interaction)
    {
        playerCharacter.Attack(interaction.ToEAttackButton(), interaction.ToEAttackCommand());
    }

    private void OnInteract<T>(T interaction)
    {
        Debug.Log("player wants to interact!");
    }

}
