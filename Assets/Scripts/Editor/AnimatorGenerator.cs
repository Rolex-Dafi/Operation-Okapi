using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public struct AnimatorTransitionProperties
{
    public AnimatorConditionMode condition;
    public float conditionThreshold;
    public string conditionParameter;
    public bool hasExit;
    public float exitTime;
    public float duration;

    public AnimatorTransitionProperties(AnimatorConditionMode condition, float conditionThreshold, string conditionParameter, 
        bool hasExit = false, float exitTime = 1, float duration = 0)
    {
        this.condition = condition;
        this.conditionThreshold = conditionThreshold;
        this.conditionParameter = conditionParameter;
        this.hasExit = hasExit;
        this.exitTime = exitTime;
        this.duration = duration;
    }
}

/// <summary>
/// Generates an animator controller for one character. Expects animation
/// clips to already be present in the folder specified by the character name.
/// </summary>
public class AnimatorGenerator
{
    private string characterName;
    private readonly string redirection = "redirection";

    public AnimatorGenerator(string characterName)
    {
        this.characterName = characterName;
    }

    /// <summary>
    /// Generates an animator controller for the given character. The state machine logic is hardcoded here.
    /// </summary>
    /// <returns></returns>
    public void GenerateAnimator()
    {
        string dir = string.Join("/", new string[] {
            GFXUtility.characterAnimationsDirectory,
            characterName
        });

        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        if (!dirInfo.Exists)
        {
            Debug.LogWarning("Directory " + dir + " not found");
        }

        // create controller
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(dir + "/" + characterName + ".controller");

        // add parameters
        foreach (EAnimationParameter parameter in Enum.GetValues(typeof(EAnimationParameter)))
        {
            controller.AddParameter(parameter.ToString(), parameter.ToType());
        }

        // add statemachines
        Dictionary<EStateMachine, AnimatorStateMachine> stateMachines = new Dictionary<EStateMachine, AnimatorStateMachine>
        {
            { EStateMachine.root, controller.layers[0].stateMachine }
        };
        stateMachines.Add(EStateMachine.move, stateMachines[EStateMachine.root].AddStateMachine(EStateMachine.move.ToString()));
        stateMachines.Add(EStateMachine.attack, stateMachines[EStateMachine.root].AddStateMachine(EStateMachine.attack.ToString()));

        // add states with blend trees
        // add an empty state for the attack sm
        Dictionary<EAbility, AnimatorState> states = new Dictionary<EAbility, AnimatorState>();
        foreach (EAbility ability in Enum.GetValues(typeof(EAbility)))
        {
            if (ability == EAbility.NDEF) continue;

            states.Add(ability, CreateBlendTree(controller, stateMachines[ability.ToEStateMachine()], ability.ToString()));
        }
        // redirection state for attacks
        AnimatorState redirectionState = stateMachines[EStateMachine.attack].AddState(redirection);
        // set deafault states for each sm
        stateMachines[EStateMachine.root].defaultState = states[EAbility.idle];
        stateMachines[EStateMachine.move].defaultState = states[EAbility.idle];
        stateMachines[EStateMachine.attack].defaultState = redirectionState;

        // add transitions
        // move SM
        AddTransition(states[EAbility.idle], states[EAbility.walk],
            new AnimatorTransitionProperties(AnimatorConditionMode.Greater, .01f, EAnimationParameter.speed.ToString()));
        AddTransition(states[EAbility.walk], states[EAbility.idle],
            new AnimatorTransitionProperties(AnimatorConditionMode.Less, .01f, EAnimationParameter.speed.ToString()));

        // attack SM
        // from movement sm
        AddTransition(states[EAbility.idle], redirectionState,
            new AnimatorTransitionProperties(AnimatorConditionMode.If, 0, EAnimationParameter.attack.ToString()));
        AddTransition(states[EAbility.walk], redirectionState,
            new AnimatorTransitionProperties(AnimatorConditionMode.If, 0, EAnimationParameter.attack.ToString()));
        // from redirection state - TODO - change this to a loop for more attacks later !!
        AddAttackTransition(redirectionState, states[EAbility.melee], stateMachines[EStateMachine.move], 0);
        AddAttackTransition(redirectionState, states[EAbility.ranged], stateMachines[EStateMachine.move], 1);

        // dash
        AddTriggerTransition(states[EAbility.idle], states[EAbility.dash], stateMachines[EStateMachine.move], EAnimationParameter.dash.ToString());

        // hit and death
        AddAnyTriggerTransition(stateMachines[EStateMachine.root], states[EAbility.hit], 
            stateMachines[EStateMachine.move], EAnimationParameter.hit.ToString());
        AddAnyTriggerTransition(stateMachines[EStateMachine.root], states[EAbility.death],
            stateMachines[EStateMachine.move], EAnimationParameter.death.ToString());

    }

    private void AddTransition(AnimatorState from, AnimatorState to, AnimatorTransitionProperties properties)
    {
        AnimatorStateTransition transition = from.AddTransition(to);
        transition.AddCondition(properties.condition, properties.conditionThreshold, properties.conditionParameter);
        SetUpTransitionProperties(transition, properties.hasExit, properties.exitTime, properties.duration);
    }

    private void AddAttackTransition(AnimatorState redirection, AnimatorState to, AnimatorStateMachine returnTo, int attackNum)
    {
        AnimatorStateTransition transition = redirection.AddTransition(to);
        transition.AddCondition(AnimatorConditionMode.Equals, attackNum, EAnimationParameter.attackNumber.ToString());
        SetUpTransitionProperties(transition);
        transition = to.AddTransition(returnTo);
        SetUpTransitionProperties(transition, true);

    }

    private void AddTriggerTransition(AnimatorState from, AnimatorState to, AnimatorStateMachine returnTo, string trigger)
    {
        AnimatorStateTransition transition = from.AddTransition(to);
        transition.AddCondition(AnimatorConditionMode.If, 0, trigger);
        SetUpTransitionProperties(transition);
        transition = to.AddTransition(returnTo);
        SetUpTransitionProperties(transition, true);
    }

    private void AddAnyTriggerTransition(AnimatorStateMachine root, AnimatorState to, AnimatorStateMachine returnTo, string trigger)
    {
        AnimatorStateTransition transition = root.AddAnyStateTransition(to);
        transition.AddCondition(AnimatorConditionMode.If, 0, trigger);
        SetUpTransitionProperties(transition);
        transition = to.AddTransition(returnTo);
        SetUpTransitionProperties(transition, true);
    }

    private void SetUpTransitionProperties(AnimatorStateTransition transition, bool hasExit = false, float exitTime = 1, float duration = 0)
    {
        transition.duration = duration;
        transition.hasExitTime = hasExit;
        transition.exitTime = exitTime;
    }

    private AnimatorState CreateBlendTree(AnimatorController controller, AnimatorStateMachine subSM, string name)
    {
        string clipDirectory = string.Join("/", new string[] { GFXUtility.characterAnimationsDirectory, characterName, name });

        if (!Directory.Exists(clipDirectory))
        {
            Debug.LogWarning("Animation clips for blend tree " + name + " in directory " + clipDirectory + " not found. Directory doesn't exist.");
            return null;
        }

        BlendTree blendTree = new BlendTree
        {
            name = name,
            hideFlags = HideFlags.HideInHierarchy,
            blendType = BlendTreeType.SimpleDirectional2D,

            blendParameter = EAnimationParameter.directionX.ToString(),
            blendParameterY = EAnimationParameter.directionY.ToString()
        };

        foreach (EDirection direction in Enum.GetValues(typeof(EDirection)))
        {
            if (direction == EDirection.NDEF) continue;

            string clipPath = clipDirectory + "/" + string.Join("_", new string[] { 
                characterName,
                name,
                direction.ToString() 
            })  + ".anim";
            AnimationClip clip = (AnimationClip)AssetDatabase.LoadAssetAtPath(clipPath, typeof(AnimationClip));
            blendTree.AddChild(clip, direction.ToVector2());
        }

        AssetDatabase.AddObjectToAsset(blendTree, controller);

        AnimatorState state = subSM.AddState(name);
        state.motion = blendTree;

        return state;
    }


}
