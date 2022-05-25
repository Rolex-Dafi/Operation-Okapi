using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private AggressiveCharacter enemyCharacter;

    void Start()
    {
        enemyCharacter = GetComponent<AggressiveCharacter>();
        enemyCharacter.Init(5, 0, 10);
    }

}
