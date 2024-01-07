using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for the final boss.
/// </summary>
public class ExCharacter : EnemyCharacter
{
    private DialogueUI dialogueUI;

    private bool triedShooting;
    
    public override void TakeDamage(int amount)
    {
        if (dialogueUI == null)
        {
            dialogueUI = FindObjectOfType<DialogueUI>();
        }

        if (amount == 1) // !!! only works if player ranged atk deals exactly 1 dmg and melee != 1
        {
            if (!triedShooting && dialogueUI != null)
            {
                StartCoroutine(
                    dialogueUI.PlaySingleLine("Ha! To get to the very top, you need to get up close and personal!"));
            }

            triedShooting = true;
            return;
        }

        base.TakeDamage(amount);
    }
}
