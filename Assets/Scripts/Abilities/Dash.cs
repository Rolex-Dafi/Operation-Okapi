using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash
{
    private AggressiveCharacter character;

    private DashScriptableObject data;

    private float lastTimeUsed;
    private int numUsedInChain;
    private bool currentlyDashing;


    public Dash(AggressiveCharacter character, DashScriptableObject data)
    {
        this.character = character;
        this.data = data;

        lastTimeUsed = 0;
        numUsedInChain = 0;
        currentlyDashing = false;
    }

    public void OnBegin()
    {
        if (currentlyDashing) return;

        // are we currently chaining dashes?
        bool inChain = (numUsedInChain < data.maxNumChained) && (Time.time - lastTimeUsed < data.deltaBeforeMax);
        float delta = inChain ? data.deltaBeforeMax : data.deltaAfterMax;

        if (Time.time - lastTimeUsed > delta)
        {
            currentlyDashing = true;

            // play animation
            // TODO change anim parameter for dash from trigger to bool

            // start moving
            //character.StartCoroutine(OnContinue());

            numUsedInChain = inChain ? numUsedInChain + 1 : 1;
        }

    }

    public IEnumerator OnContinue()
    {
        while (currentlyDashing)
        {
            // TODO move the character in specified direction
            // call OnEnd and break after moving the specified distance - or after colliding with a wall !

            yield return new WaitForFixedUpdate();
        }
    }


    public void OnEnd()
    {
        // stop playing the animation

        // stop moving

        currentlyDashing = false;
        lastTimeUsed = Time.time;
    }
}
