using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Ability
{
    private float lastTimeUsed;
    private int numUsedInChain;
    private bool currentlyDashing;
    public bool CurrentlyDashing { get => currentlyDashing; private set => currentlyDashing = value; }

    public DashSO Data { get => (DashSO)data; protected set => data = value; }

    public Dash(CombatCharacter character, DashSO data) : base(character, data, EAbilityType.dash)
    {
        lastTimeUsed = 0;
        numUsedInChain = 0;
        CurrentlyDashing = false;
    }


    public override void OnBegin()
    {
        if (CurrentlyDashing) return;

        // are we currently chaining dashes?
        bool inChain = (numUsedInChain < Data.maxNumChained);
        float delta = inChain ? Data.deltaBeforeMax : Data.deltaAfterMax;

        if (Time.time - lastTimeUsed > delta)
        {
            CurrentlyDashing = true;

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
        float distanceTravelled = 0;

        while (CurrentlyDashing)
        {
            // TODO call OnEnd() after colliding with a wall as well!
            // for some reason this already seems to be working ??
            // it'll probably stop working after disabling the collider to dash through enemies though

            if (distanceTravelled > Data.distance) { 
                OnEnd();
                break;
            }

            Vector2 step = direction * Time.fixedDeltaTime * Data.speed;
            character.RB.MovePosition(character.RB.position + step);

            distanceTravelled += step.magnitude;

            yield return new WaitForFixedUpdate();
        }
    }


    public override void OnEnd()
    {
        // stop playing the animation
        character.Animator.SetBool(EAnimationParameter.dashing.ToString(), false);

        // stop moving
        CurrentlyDashing = false;
        lastTimeUsed = Time.time;
    }
}
