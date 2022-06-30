using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash
{
    private CombatCharacter character;

    private DashSO data;

    private float lastTimeUsed;
    private int numUsedInChain;
    private bool currentlyDashing;
    public bool CurrentlyDashing { get => currentlyDashing; private set => currentlyDashing = value; }

    public Dash(CombatCharacter character, DashSO data)
    {
        this.character = character;
        this.data = data;

        lastTimeUsed = 0;
        numUsedInChain = 0;
        CurrentlyDashing = false;
    }


    public void OnBegin()
    {
        if (CurrentlyDashing) return;

        // are we currently chaining dashes?
        bool inChain = (numUsedInChain < data.maxNumChained);
        float delta = inChain ? data.deltaBeforeMax : data.deltaAfterMax;

        if (Time.time - lastTimeUsed > delta)
        {
            CurrentlyDashing = true;

            // play animation
            // TODO change anim parameter for dash from trigger to bool
            character.Animator.SetBool(EAnimationParameter.dashing.ToString(), true);

            // TODO allow to dash through enemies (maybe objects even)

            // start moving
            character.StartCoroutine(OnContinue());

            numUsedInChain = inChain ? numUsedInChain + 1 : 1;
        }

    }

    public IEnumerator OnContinue()
    {
        // dash in isometric coordinates !
        Vector2 direction = character.Facing.CartesianToIsometric().normalized;
        float distanceTravelled = 0;

        while (CurrentlyDashing)
        {
            // TODO call OnEnd() after colliding with a wall as well!
            // for some reason this already seems to be working ??

            if (distanceTravelled > data.distance) { 
                OnEnd();
                break;
            }

            Vector2 step = direction * Time.fixedDeltaTime * data.speed;
            character.RB.MovePosition(character.RB.position + step);

            distanceTravelled += step.magnitude;

            yield return new WaitForFixedUpdate();
        }
    }


    public void OnEnd()
    {
        // stop playing the animation
        character.Animator.SetBool(EAnimationParameter.dashing.ToString(), false);

        // stop moving
        CurrentlyDashing = false;
        lastTimeUsed = Time.time;
    }
}
