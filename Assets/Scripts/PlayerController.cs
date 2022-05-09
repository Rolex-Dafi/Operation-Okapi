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

        playerInput.dashButtonDown.AddListener(OnDash);
        playerInput.meleeButtonDown.AddListener(OnMeleeAttack);
        playerInput.rangedButtonDown.AddListener(OnRangedAttack);
        playerInput.specialButtonDown.AddListener(OnSpecialAttack);
        playerInput.interactButtonDown.AddListener(OnInteract);
    }

    private void FixedUpdate()
    {
        // movement        
        playerCharacter.Move(playerInput.move.normalized);
        
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

    private void OnMeleeAttackEnd()
    {
        Debug.Log("player ended a melee attack!");
    }

    private void OnRangedAttack()
    {
        Debug.Log("player wants to perform a ranged attack!");
        playerCharacter.RangedAttack();
    }

    private void OnRangedAttackEnd()
    {
        Debug.Log("player ended a ranged attack!");
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
