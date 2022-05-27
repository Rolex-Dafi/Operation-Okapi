using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateMachine : StateMachineBehaviour
{
    private AggressiveCharacter aggressiveCharacter;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!aggressiveCharacter) aggressiveCharacter = animator.GetComponent<AggressiveCharacter>();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateinfo, int layerindex)
    {
        aggressiveCharacter.ResetMovementSpeed();
    }

}
