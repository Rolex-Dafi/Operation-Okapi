using System.Collections;
using UnityEngine;

/// <summary>
/// Class for the dash ability.
/// </summary>
public class Dash : Ability
{
    private HitBoxController hitBoxController; // if dash does damage - the collider also needs to be enabled in the animation
    
    private float lastTimeUsed;
    private int numUsedInChain;
    public DashSO Data { get => (DashSO)data; protected set => data = value; }

    private readonly int characterLayerID;
    
    /// <summary>
    /// Creates a dash instance.
    /// </summary>
    /// <param name="character">The character to which the attack belongs</param>
    /// <param name="data">The dash data</param>
    public Dash(CombatCharacter character, DashSO data) : base(character, data, EAbilityType.dash)
    {
        lastTimeUsed = 0;
        numUsedInChain = 0;
        characterLayerID = character.gameObject.layer;
        
        hitBoxController = character.GetComponentInChildren<HitBoxController>();
    }

    public override void OnBegin()
    {
        if (InUse) return;

        // reset if deltaAfterMax time passed
        if (Time.time - lastTimeUsed > Data.deltaAfterMax)
        {
            numUsedInChain = 0;
        }

        // if deltaBeforeMax time passed, we can chain dashes until maxNumChained
        if (Time.time - lastTimeUsed > Data.deltaBeforeMax && numUsedInChain < Data.maxNumChained)
        {
            base.OnBegin();

            // play animation
            character.Animator.SetBool(EAnimationParameter.dashing.ToString(), true);

            // start moving
            character.StartCoroutine(OnContinue());

            ++numUsedInChain;
        }
    }

    public override IEnumerator OnContinue()
    {
        // dash in isometric coordinates !
        Vector2 direction = character.Facing.CartesianToIsometric().normalized;

        // ignore obstacles when dashing - this includes enemies/the player - set in Layer Collision Matrix in Project Settings
        character.gameObject.layer = LayerMask.NameToLayer(Utility.ignoreObstaclesLayer);
        
        // if dash does damage, init hitbox
        if (Data.damage > 0) hitBoxController.Init(Data);
        
        while (InUse)
        {
            yield return character.RB.AddForceCustom(direction, Data.distance, Data.speed, OnEnd);
        }
    }


    public override void OnEnd()
    {
        // stop playing the animation
        character.Animator.SetBool(EAnimationParameter.dashing.ToString(), false);

        // set physics layer back to normal
        character.gameObject.layer = characterLayerID;
        
        // stop moving
        InUse = false;
        lastTimeUsed = Time.time;
    }
}
