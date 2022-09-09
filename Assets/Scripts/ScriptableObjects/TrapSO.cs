using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "TrapData", menuName = "ScriptableObjects/Trap")]
public class TrapSO : AbilitySO
{
    [Range(0, 2)] public float activationTime; // the time it takes for the character to activate the trap
    
    public Vector3 spawnPosition;
    public TrapController trapControllerPrefab;

    public Trap GetTrap(CombatCharacter character)
    {
        return new Trap(character, this);
    }
}
