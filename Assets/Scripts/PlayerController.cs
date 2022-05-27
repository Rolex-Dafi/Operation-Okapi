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

    private bool rotatingWithMouse;
    private Vector2 mousePositionWorld;

    public bool RotatingWithMouse { get => rotatingWithMouse; set => rotatingWithMouse = value; }
    public Vector2 MousePositionWorld { get => mousePositionWorld; private set => mousePositionWorld = value; }

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

        if (rotatingWithMouse) RotateWithMouse();
    }

    private void RotateWithMouse()
    {
        // get world mouse position
        mousePositionWorld = Camera.main.ScreenToWorldPoint(playerInput.mousePosition);

        // rotate the character
        playerCharacter.Rotate((mousePositionWorld - transform.position.ToVector2()).normalized);

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
