using FMODUnity;
using System.Collections;
using UnityEngine;

/// <summary>
/// Wrapper class for all characters' abilities.
/// </summary>
public abstract class Ability
{
    protected CombatCharacter character;
    protected AbilitySO data;

    protected float _lastUsed;

    public EAbilityType type;

    public bool InUse { get; set; }
    public bool OnCoolDown { get => Time.time - _lastUsed < data.coolDown; }

    protected Ability(CombatCharacter character, AbilitySO data, EAbilityType type)
    {
        this.character = character;
        this.data = data;
        this.type = type;
    }

    /// <summary>
    /// Begins performing the ability.
    /// </summary>
    public virtual void OnBegin() 
    {
        // play on begin sound from SO if assigned
        if (!data.onBeginSound.IsNull) RuntimeManager.PlayOneShot(data.onBeginSound.Guid);

        _lastUsed = Time.time;
        InUse = true;
    }

    /// <summary>
    /// Updates the performing of the ability if the ability is in use.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator OnContinue() 
    {
        yield return null;
    }

    /// <summary>
    /// Ends the performing of the ability.
    /// </summary>
    public virtual void OnEnd() { }

}
