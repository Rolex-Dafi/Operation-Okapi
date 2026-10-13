using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    /// <summary>
    /// Should only be called at the beginning of the game - instantiates the player prefab under the specified transform
    /// and initializes the player character.
    /// </summary>
    /// <param name="prefab">player prefab</param>
    /// <param name="parent">parent transform to instantiate under, transform of this object if empty</param>
    /// <returns>The instantiated and initiated instance of the player character</returns>
    public PlayerCharacter SpawnPlayerAndInit(PlayerCharacter prefab, Transform parent = default)
    {
        var parentTransform = parent == null ? transform : parent;
        var instance = Instantiate(prefab, parentTransform);

        // initialize everything inside player prefab
        instance.GetComponent<PlayerController>().Init();

        // set camera to follow the player
        FindObjectOfType<CinemachineVirtualCamera>().Follow = instance.transform;

        // player invisible after initialization -> level manager should enable it by calling PlacePlayer
        instance.gameObject.SetActive(false);
        
        return instance;
    }

    /// <summary>
    /// Places the player instance under the specified parent. Assumes the player has already been instantiated and
    /// initialized. Should be called at the beginning of each room after it has been generated.
    /// </summary>
    /// <param name="player">the player instance to spawn</param>
    /// <param name="position">the position at which it should be spawned</param>
    public static void PlacePlayer(PlayerCharacter player, Vector3 position)
    {
        player.transform.position = position;
        player.gameObject.SetActive(true);
    }
}
