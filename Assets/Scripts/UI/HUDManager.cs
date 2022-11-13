using UnityEngine;

/// <summary>
/// 
/// </summary>
public class HUDManager : MonoBehaviour
{
    [SerializeField] private ResourceUI money;

    [SerializeField] private HealthUI health;

    //private GameManager gameManager;

    /// <summary>
    /// Should be called from game manager after scene loaded.
    /// </summary>
    /// <param name="gameManager"></param>
    /// <param name="playerCharacter">The current player character instance</param>
    public void Init(GameManager gameManager, PlayerCharacter playerCharacter)
    {
        money.Init(playerCharacter.Money);
        health.Init(gameManager, playerCharacter.Inventory);
    }

}
