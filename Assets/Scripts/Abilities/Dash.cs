using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Ability
{
    private float lastTimeUsed;
    private int numUsedInChain;
    public DashSO Data { get => (DashSO)data; protected set => data = value; }

    private readonly int characterLayerID;
    
    public Dash(CombatCharacter character, DashSO data) : base(character, data, EAbilityType.dash)
    {
        lastTimeUsed = 0;
        numUsedInChain = 0;
        characterLayerID = character.gameObject.layer;
    }


    public override void OnBegin()
    {
        if (InUse) return;

        // are we currently chaining dashes?
        bool inChain = (numUsedInChain < Data.maxNumChained);
        float delta = inChain ? Data.deltaBeforeMax : Data.deltaAfterMax;

        if (Time.time - lastTimeUsed > delta)
        {
            base.OnBegin();

            // play animation
            character.Animator.SetBool(EAnimationParameter.dashing.ToString(), true);

            // TODO allow to dash through enemies and objects

            // start moving
            character.StartCoroutine(OnContinue());

            numUsedInChain = inChain ? numUsedInChain + 1 : 1;
        }

    }

    public override IEnumerator OnContinue()
    {
        // dash in isometric coordinates !
        Vector2 direction = character.Facing.CartesianToIsometric().normalized;

        // ignore obstacles when dashing - this includes enemies/the player - set in Layer Collision Matrix in Project Settings
        character.gameObject.layer = LayerMask.NameToLayer(Utility.ignoreObstaclesLayer);
        
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
