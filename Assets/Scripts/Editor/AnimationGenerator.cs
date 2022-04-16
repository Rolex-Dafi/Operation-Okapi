using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Generator for animation clips. The clips are placed in a hierarchy according to file names. 
/// Expects .png files on input, with the following naming convention:
/// <character-name>_<animation-name>_<direction>_<frame-number>.png
/// </summary>
public class AnimationGenerator
{



    public AnimationClip GenerateAnimationClip(int frameRate, bool loop)
    {
        AnimationClip clip = new AnimationClip
        {
            frameRate = frameRate
        };
        AnimationClipSettings settings = new AnimationClipSettings
        {
            loopTime = loop
        };

        AnimationUtility.SetAnimationClipSettings(clip, settings);


        return clip;
    }


}
