using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// should be on a trigger 2d
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private bool startFromTrigger; // if this is false, the some function needs to start the dialogue manually
    [SerializeField] private LevelSO levelSo;
    [SerializeField] private int passageIndex;
    
    private bool active;
    private bool alreadyTriggered;

    private Passage dialogue;
    private DialogueUI dialogueUI;

    private UnityAction onDialogueEnd;

    private void Awake()
    {
        if (startFromTrigger)
        {
            // TODO this is a hack, won't work for many dialogue modifications, fix later
            Init(FindObjectOfType<DialogueUI>(), levelSo.levelDialogue.rooms[0].passages[passageIndex]);
        }
    }

    // should be called when spawning the room - from lvl man
    public void Init(DialogueUI dialogueUI, Passage dialogue)
    {
        this.dialogueUI = dialogueUI;
        this.dialogue = dialogue;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!startFromTrigger || alreadyTriggered) return;

        if (!col.CompareTag(Utility.playerTagAndLayer)) return;  // ignore non-players

        if (!col.TryGetComponent<PlayerCharacter>(out _)) return; // ignore projectiles
            
        StartDialogue();
    }
    
    public void StartDialogue(UnityAction onEnd = null)
    {
        alreadyTriggered = true;
        onDialogueEnd = onEnd;
        
        // play the dialogue
        StartCoroutine(WriteDialogue());
    }
    
    private IEnumerator WriteDialogue()
    {
        yield return dialogueUI.Open();
        foreach (var dialogueLine in dialogue.lines)
        {
            // wait for each line to finish
            yield return dialogueUI.PlayLine(dialogueLine);
            
            // and for the player to press anything 
            yield return new WaitUntil(() => Input.anyKeyDown);
        }
        
        yield return dialogueUI.Close();
        
        // any on end fc after finishing dialogue should be triggered here
        onDialogueEnd?.Invoke();
    }
}
