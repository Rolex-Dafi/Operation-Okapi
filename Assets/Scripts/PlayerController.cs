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

    private void Start()
    {
        // player character
        playerCharacter = GetComponent<PlayerCharacter>();
        playerCharacter.Init();

        // player input
        playerInput = GetComponent<PlayerInput>();
        playerInput.Init();

        // axis events
        playerInput.moveEvent.AddListener(OnMove);

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

    private void OnMove(Vector2 move)
    {
        playerCharacter.Move(move.normalized);
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
