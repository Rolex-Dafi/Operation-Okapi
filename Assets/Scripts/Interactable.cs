using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    private bool playerInRange;

    [HideInInspector] public UnityEvent onInteractPressed;
    
    private TooltipUI tooltipInstance;

    public void Init(string message = "Press E to Interact") // TODO take into account different inputs - ex. gamepad
    {
        onInteractPressed = new UnityEvent();
        
        // find player input and subscribe to it
        var playerInput = FindObjectOfType<PlayerInput>();
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

    private void TryInteract()
    {
        if (playerInRange)
        {
            Debug.Log("Trying to interact with interactable", gameObject);
            onInteractPressed.Invoke();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Utility.playerTagAndLayer))
        {
            Debug.Log("entering interactable trigger", gameObject);
            tooltipInstance.ShowToolTip(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Utility.playerTagAndLayer))
        {
            Debug.Log("leaving interactable trigger", gameObject);
            tooltipInstance.ShowToolTip(false);
            playerInRange = false;
        }
    }

}
