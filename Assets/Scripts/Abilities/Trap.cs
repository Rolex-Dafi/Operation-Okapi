using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Ability
{
    private TrapController trapController;
    
    public TrapSO Data => (TrapSO)data;

    public Trap(CombatCharacter character, AbilitySO data, EAbilityType type = EAbilityType.special) : base(character, data, type)
    {
        trapController = Object.Instantiate(Data.trapControllerPrefab, Data.spawnPosition, Quaternion.identity);
        trapController.Init(Data);
    }

    public override void OnBegin()
    {
        trapController.ActivateTrap();
    }

}
