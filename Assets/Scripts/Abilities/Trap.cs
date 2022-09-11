using UnityEngine;

/// <summary>
/// Class for the trap ability.
/// </summary>
public class Trap : Ability
{
    private TrapController trapController;
    
    public TrapSO Data => (TrapSO)data;

    /// <summary>
    /// Creates a trap instance and instantiates a trap in the scene according to the specified parameters.
    /// </summary>
    /// <param name="character">The character to which the attack belongs</param>
    /// <param name="data">The trap data</param>
    /// <param name="type"></param>
    public Trap(CombatCharacter character, AbilitySO data, EAbilityType type = EAbilityType.special) : base(character, data, type)
    {
        // spawn it under the current room so that it gets destroyed when exiting the room
        trapController = Object.Instantiate(Data.trapControllerPrefab, Data.spawnPosition, Quaternion.identity, LevelManager.CurrentRoomTransform);
        trapController.Init(Data);
    }

    public override void OnBegin()
    {
        trapController.ActivateTrap();
        base.OnBegin();
        OnEnd();
    }

    public override void OnEnd()
    {
        InUse = false;
    }
}
