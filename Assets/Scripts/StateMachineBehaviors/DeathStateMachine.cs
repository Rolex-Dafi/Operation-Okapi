using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State machine for death state of all characters.
/// </summary>
public class DeathStateMachine : StateMachineBehaviour
{
    private CombatCharacter combatCharacter;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!combatCharacter) combatCharacter = animator.GetComponent<CombatCharacter>();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
    {
        combatCharacter.CleanUp();
    }

}
