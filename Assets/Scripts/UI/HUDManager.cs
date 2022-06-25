using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    // TODO change this to find the player character after it's been spawned
    // probably set this up (and spawn this) from the game manager later 
    [SerializeField] private PlayerCharacter playerCharacter;

    [SerializeField] private ResourceUI health;
    [SerializeField] private ResourceUI money;
    [SerializeField] private ResourceUI respect;

    private void Start()
    {
        health.Init(playerCharacter.Health);
        money.Init(playerCharacter.Money);
        respect.Init(playerCharacter.Respect);
    }

}
