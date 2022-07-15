using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Ability
{
    protected CombatCharacter character;
    protected AbilitySO data;

    public EAbilityType type;

    protected Ability(CombatCharacter character, AbilitySO data, EAbilityType type)
    {
        this.character = character;
        this.data = data;
        this.type = type;
    }

    public virtual void OnBegin() 
    {
        // play sound from the scriptable object for this ability
        FMODUnity.RuntimeManager.PlayOneShot(data.sound.Path);
    }

    public virtual IEnumerator OnContinue() 
    {
        yield return null;
    }

    public virtual void OnEnd() { }

}
