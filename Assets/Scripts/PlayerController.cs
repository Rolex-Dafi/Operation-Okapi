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

    public bool Aiming { get => aiming; set => aiming = value; }

    public void Init()
    {
        // player character
        playerCharacter = GetComponent<PlayerCharacter>();
        playerCharacter.Init();

        // player input
        playerInput = GetComponent<PlayerInput>();
        playerInput.Init();

        // button down events
        foreach (EButtonDown interaction in Enum.GetValues(typeof(EButtonDown)))
        {
            UnityAction<EButtonDown> action = interaction switch
            {
                EButtonDown.Dash => OnDash,
                EButtonDown.Interact => OnInteract,
                _ => OnAttack
            };

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
        }
        else
        {
            // player faces in the direction of the mouse
            direction = (Camera.main.ScreenToWorldPoint(playerInput.mousePosition).ToVector2() - transform.position.ToVector2());
        }

        // rotate the character
        playerCharacter.Rotate(direction.normalized);

        // rotate the aiming gfx as well
        playerCharacter.RotateAimingGFX();
    }

    private void OnDash<T>(T interaction)
    {
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
