using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AggressiveCharacter))]
public class AIController : MonoBehaviour
{
    // required components
    private AIBehaviour aiBehaviour;
    private AIPathing aiPath;
    private AggressiveCharacter character;


    void Start()
    {
        // get components
        aiBehaviour = GetComponent<AIBehaviour>();
        aiPath = GetComponent<AIPathing>();
        character = GetComponent<AggressiveCharacter>();
        character.Init(5, 0);
    }

}
