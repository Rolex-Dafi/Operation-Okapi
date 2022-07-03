using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private ResourceUI health;
    [SerializeField] private ResourceUI money;
    [SerializeField] private ResourceUI respect;


    /// <summary>
    /// Should be called from game manager after scene loaded.
    /// </summary>
    /// <param name="playerCharacter"></param>
    public void Init(PlayerCharacter playerCharacter)
    {
        health.Init(playerCharacter.Health);
        money.Init(playerCharacter.Money);
        respect.Init(playerCharacter.Respect);
    }

}
