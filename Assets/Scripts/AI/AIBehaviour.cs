using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    // TODO generate these Transforms based on level instead of presetting them 
    // // -> maybe hold these in the ai spawner and select only the closest points to this enemy for each enemy?
    // also change these when transitioning from searching state back to patroll depending on current location
    public Transform[] patrollPoints;


    // fix this hack later
    private AIPathing aiPathing;

    private EAIState currentState;

    private int nextPatrollPoint = 0;
    private bool movingToNextPatrolPoint = false;

    private void Start()
    {
        aiPathing = GetComponent<AIPathing>();

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
        if (movingToNextPatrolPoint) return;

        Debug.Log("Moving to patroll point " + nextPatrollPoint);
        aiPathing.MoveTo(patrollPoints[nextPatrollPoint], PatrollPointReached);
        movingToNextPatrolPoint = true;
    }

    private void PatrollPointReached()
    {
        Debug.Log("Patroll point idx " + nextPatrollPoint + " reached.");

        movingToNextPatrolPoint = false;
        ++nextPatrollPoint;
        nextPatrollPoint %= patrollPoints.Length;
    }

}
