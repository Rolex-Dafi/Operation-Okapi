using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// should be on a trigger 2d
public class DialogueTrigger : MonoBehaviour
{
    private bool active;
    private bool alreadyTriggered;

    private Passage dialogue;
    private DialogueUI dialogueUI;

    // should be called when spawning the room - from lvl man
    public void Init(DialogueUI dialogueUI, Passage dialogue)
    {
        this.dialogueUI = dialogueUI;
        this.dialogue = dialogue;
    }

    public void Activate()
    {
        active = true;
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!active || alreadyTriggered) return;

        alreadyTriggered = true;
        
        // play the dialogue
        StartCoroutine(WriteDialogue());
    }

    private IEnumerator WriteDialogue()
    {
        // TODO pause the game - let ui handle tnis
        
        dialogueUI.Open();
        foreach (var dialogueLine in dialogue.lines)
        {
            // wait for each line to finish
            yield return dialogueUI.PlayLine(dialogueLine);
            
            // and for the player to press enter - TODO let ui handle this
        }
        
        // TODO unpause - let ui handle tnis
    }
}
