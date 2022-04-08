using UnityEngine;

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
        playerCharacter = GetComponent<PlayerCharacter>();
        playerCharacter.Init();
        playerInput = GetComponent<PlayerInput>();
        playerInput.Init();

        playerInput.dashEvent.AddListener(OnDash);
        playerInput.meleeAttackEvent.AddListener(OnMeleeAttack);
        playerInput.rangedAttackEvent.AddListener(OnRangedAttack);
        playerInput.specialAttackEvent.AddListener(OnSpecialAttack);
        playerInput.interactEvent.AddListener(OnInteract);
    }

    private void FixedUpdate()
    {
        // movement
        if (playerInput.move != Vector2.zero)
        {
            playerCharacter.Move(playerInput.move.normalized);
        }
    }

    private void OnDash()
    {
        Debug.Log("player wants to dash!");
        playerCharacter.Dash();
    }

    private void OnMeleeAttack()
    {
        Debug.Log("player wants to perform a melee attack!");
        playerCharacter.MeleeAttack();
    }

    private void OnRangedAttack()
    {
        Debug.Log("player wants to perform a ranged attack!");
        playerCharacter.RangedAttack();
    }

    private void OnSpecialAttack()
    {
        Debug.Log("player wants to perform a special attack!");
        playerCharacter.SpecialAttack();
    }

    private void OnInteract()
    {
        Debug.Log("player wants to interact!");
    }

}
