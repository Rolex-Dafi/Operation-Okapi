using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    // TODO generate these Transforms based on level instead of presetting them
    // also change these when transitioning from searching state back to patroll depending on current location
    [SerializeField] private Transform[] patrollPoints;


    private EAIState currentState;

    private void Start()
    {
        currentState = EAIState.Patrolling;
    }

    private void Update()
    {
        switch (currentState)
        {
            case EAIState.Patrolling:
                Patroll();
                break;
            case EAIState.Pursuing:
                break;
            case EAIState.Searching:
                break;
            case EAIState.Attacking:
                break;
            default:
                break;
        }
    }

    private void Patroll()
    {

    }

}
