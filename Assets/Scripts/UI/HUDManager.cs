using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    //[SerializeField] private ResourceUI health;
    [SerializeField] private ResourceUI money;
    [SerializeField] private ResourceUI respect;

    [SerializeField] private HealthUI health;

    //private GameManager gameManager;

    /// <summary>
    /// Should be called from game manager after scene loaded.
    /// </summary>
    /// <param name="gameManager"></param>
    /// <param name="playerCharacter"></param>
    public void Init(GameManager gameManager, PlayerCharacter playerCharacter)
    {
        //this.gameManager = gameManager;
        
        money.Init(playerCharacter.Money);
        respect.Init(playerCharacter.Respect);
        
        health.Init(gameManager, playerCharacter.Inventory);
    }

}
