using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public PlayerCharacter SpawnPlayer(PlayerCharacter prefab)
    {
        PlayerCharacter instance = Instantiate(prefab, transform);

        // initialize everything inside player prefab
        instance.GetComponent<PlayerController>().Init();

        // set camera to follow the player
        FindObjectOfType<CinemachineVirtualCamera>().Follow = instance.transform;

        return instance;
    }
}
