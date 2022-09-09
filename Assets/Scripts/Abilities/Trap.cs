using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Ability
{
    private TrapController trapController;
    
    public TrapSO Data { get => (TrapSO)data; protected set => data = value; }
    
    public Trap(CombatCharacter character, AbilitySO data, EAbilityType type = EAbilityType.special) : base(character, data, type)
    {
        
    }

    public override void OnBegin()
    {
        
    }

}
