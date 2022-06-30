using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateMachine : StateMachineBehaviour
{
    private CombatCharacter aggressiveCharacter;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!aggressiveCharacter) aggressiveCharacter = animator.GetComponent<CombatCharacter>();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
    {
        aggressiveCharacter.ResetMovementSpeed();
    }

}
