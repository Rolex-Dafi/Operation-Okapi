using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Ability
{
    private TrapController trapController;
    
    public TrapSO Data => (TrapSO)data;

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
