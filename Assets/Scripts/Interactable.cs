using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class for game objects with which the player can interact by pressing the interact button.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    private bool playerInRange;

    [HideInInspector] public UnityEvent onInteractPressed;
    
    private TooltipUI tooltipInstance;
    
    public PlayerCharacter PlayerCharacter { get; private set; }

    public void Init(string message) // TODO take into account different inputs - ex. gamepad
    {
        onInteractPressed = new UnityEvent();
        
        // find player input and subscribe to it
        var playerInput = FindObjectOfType<PlayerInput>(true);
        playerInput.buttonDownEvents[EButtonDown.Interact].AddListener((_) => TryInteract());
        
        // add tooltip
        var gameManager = FindObjectOfType<GameManager>();
        tooltipInstance = Instantiate(
            gameManager.tooltipUIPrefab, 
            transform.position, 
            quaternion.identity, 
            gameManager.worldSpaceCanvas.transform
            );
        tooltipInstance.Init(message); 
    }

    /// <summary>
    /// Set the text of the tooltip of this interactable, if present.
    /// </summary>
    /// <param name="message"></param>
    public void SetTooltip(string message)
    {
        if (tooltipInstance == null) return;
        
        tooltipInstance.SetText(message);
    }
    
    private void TryInteract()
    {
        if (playerInRange)
        {
            onInteractPressed.Invoke();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Utility.playerTagAndLayer))
        {
            if (other.TryGetComponent<PlayerCharacter>(out var character)) // only interact with player char, not projectiles
            {
                if (tooltipInstance == null) return;
            
                tooltipInstance.ShowToolTip(true);
                playerInRange = true;
                
                PlayerCharacter = character;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Utility.playerTagAndLayer))
        {
            if (other.TryGetComponent<PlayerCharacter>(out _)) // only interact with player char, not projectiles
            {
                if (tooltipInstance == null) return;

                tooltipInstance.ShowToolTip(false);
                playerInRange = false;
            }
        }
    }

}
