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
            controller.AddParameter(parameter.ToString(), parameter.ToAnimatorParameterType());
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
        Dictionary<EAbilityType, AnimatorState> states = new Dictionary<EAbilityType, AnimatorState>();
        foreach (EAbilityType ability in Enum.GetValues(typeof(EAbilityType)))
        {
            if (ability == EAbilityType.NDEF) continue;

            // TODO change this to be less hacky later
            string attackFramesName = ability == EAbilityType.ranged ? "active" : "";

            states.Add(ability, CreateBlendTree(controller, stateMachines[ability.ToEStateMachine()], ability.ToString(), attackFramesName));
        }
        // redirection state for attacks
        AnimatorState redirectionState = stateMachines[EStateMachine.attack].AddState(redirection);
        // startup state for aimed ranged attack
        AnimatorState rangedStartupState = CreateBlendTree(controller, stateMachines[EAbilityType.ranged.ToEStateMachine()], EAbilityType.ranged.ToString(), "startup");
        // set default states for each sm
        stateMachines[EStateMachine.root].defaultState = states[EAbilityType.idle];
        stateMachines[EStateMachine.move].defaultState = states[EAbilityType.idle];
        stateMachines[EStateMachine.attack].defaultState = redirectionState;

        // add transitions
        // move SM
        AddTransition(states[EAbilityType.idle], states[EAbilityType.walk],
            new AnimatorTransitionProperties(AnimatorConditionMode.Greater, .01f, EAnimationParameter.speed.ToString()));
        AddTransition(states[EAbilityType.walk], states[EAbilityType.idle],
            new AnimatorTransitionProperties(AnimatorConditionMode.Less, .01f, EAnimationParameter.speed.ToString()));

        // attack SM
        // from movement sm
        AddTransition(states[EAbilityType.idle], redirectionState,
            new AnimatorTransitionProperties(AnimatorConditionMode.If, 0, EAnimationParameter.attack.ToString()));
        AddTransition(states[EAbilityType.walk], redirectionState,
            new AnimatorTransitionProperties(AnimatorConditionMode.If, 0, EAnimationParameter.attack.ToString()));
        // from redirection state -
        // TODO change this to a loop for more attacks later !!
        // TODO also take into account different attack effects (i.e. for click, aim and spray attacks there'll be different states !!
        AddAttackTransition(redirectionState, states[EAbilityType.melee], stateMachines[EStateMachine.move], 0);
        AddAttackTransition(redirectionState, states[EAbilityType.ranged], stateMachines[EStateMachine.move], 1, EAttackEffect.Aim, rangedStartupState);
        // add behaviours
        states[EAbilityType.melee].AddStateMachineBehaviour<AttackStateMachine>();
        rangedStartupState.AddStateMachineBehaviour<AttackStateMachine>();

        // dash
        AddBoolTransition(states[EAbilityType.idle], states[EAbilityType.dash], stateMachines[EStateMachine.move], EAnimationParameter.dashing.ToString());
        AddBoolTransition(states[EAbilityType.walk], states[EAbilityType.dash], stateMachines[EStateMachine.move], EAnimationParameter.dashing.ToString());

        // hit and death
        AddAnyTriggerTransition(stateMachines[EStateMachine.root], states[EAbilityType.hit], 
            stateMachines[EStateMachine.move], EAnimationParameter.hit.ToString());
        AddAnyTriggerTransition(stateMachines[EStateMachine.root], states[EAbilityType.death],
            stateMachines[EStateMachine.move], EAnimationParameter.death.ToString());

    }

    private void AddTransition(AnimatorState from, AnimatorState to, AnimatorTransitionProperties properties)
    {
        AnimatorStateTransition transition = from.AddTransition(to);
        transition.AddCondition(properties.condition, properties.conditionThreshold, properties.conditionParameter);
        SetUpTransitionProperties(transition, properties.hasExit, properties.exitTime, properties.duration);
    }

    private void AddAttackTransition(AnimatorState redirection, AnimatorState to, AnimatorStateMachine returnTo, int attackNum, 
        EAttackEffect attackEffect = EAttackEffect.Click, AnimatorState startup = null)
    {
        if (attackEffect == EAttackEffect.Click)
        {
            AnimatorStateTransition transition = redirection.AddTransition(to);
            transition.AddCondition(AnimatorConditionMode.Equals, attackNum, EAnimationParameter.attackID.ToString());
            SetUpTransitionProperties(transition);
            transition = to.AddTransition(returnTo);
            SetUpTransitionProperties(transition, true);
        }
        else if (attackEffect == EAttackEffect.Aim)
        {
            AnimatorStateTransition transition = redirection.AddTransition(startup);
            transition.AddCondition(AnimatorConditionMode.Equals, attackNum, EAnimationParameter.attackID.ToString());
            SetUpTransitionProperties(transition);
            transition = startup.AddTransition(to);
            transition.AddCondition(AnimatorConditionMode.If, 0, EAnimationParameter.attackReleased.ToString());
            SetUpTransitionProperties(transition);
            transition = to.AddTransition(returnTo);
            SetUpTransitionProperties(transition, true);
        }
    }

    private void AddBoolTransition(AnimatorState from, AnimatorState to, AnimatorStateMachine returnTo, string boolean)
    {
        AnimatorStateTransition transition = from.AddTransition(to);
        transition.AddCondition(AnimatorConditionMode.If, 0, boolean);
        SetUpTransitionProperties(transition);
        transition = to.AddTransition(returnTo);
        transition.AddCondition(AnimatorConditionMode.IfNot, 0, boolean);
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

    private AnimatorState CreateBlendTree(AnimatorController controller, AnimatorStateMachine subSM, string name, string attackFramesName = "")
    {
        string clipDirectory = string.Join("/", new string[] { GFXUtility.characterAnimationsDirectory, characterName, name });
        clipDirectory += attackFramesName == "" ? "" : "/" + attackFramesName;

        if (!Directory.Exists(clipDirectory))
        {
            Debug.LogWarning("Animation clips for blend tree " + name + " in directory " + clipDirectory + " not found. Directory doesn't exist.");
            return null;
        }

        string stateName = name;
        stateName += attackFramesName == "" ? "" : "_" + attackFramesName;

        BlendTree blendTree = new BlendTree
        {
            name = stateName,
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
            });
            clipPath += attackFramesName == "" ? "" : "_" + attackFramesName;
            clipPath += ".anim";
            AnimationClip clip = (AnimationClip)AssetDatabase.LoadAssetAtPath(clipPath, typeof(AnimationClip));
            blendTree.AddChild(clip, direction.ToVector2());
        }

        AssetDatabase.AddObjectToAsset(blendTree, controller);

        AnimatorState state = subSM.AddState(stateName);
        state.motion = blendTree;

        return state;
    }

    // TODO Replace animation clips in a blend tree
    public void ReplaceBlendTreeClips(EAbilityType ability)
    {
        string dir = string.Join("/", new string[] {
            GFXUtility.characterAnimationsDirectory,
            characterName
        });

        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(dir + "/" + characterName + ".controller");

        EStateMachine esm = ability.ToEStateMachine();

        //AnimatorStateMachine sm = controller.stat

        //BlendTree blendTree

        string clipDirectory = string.Join("/", new string[] { GFXUtility.characterAnimationsDirectory, characterName, ability.ToString() });
        foreach (EDirection direction in Enum.GetValues(typeof(EDirection)))
        {
            if (direction == EDirection.NDEF) continue;

            string clipPath = clipDirectory + "/" + string.Join("_", new string[] {
                characterName,
                ability.ToString(),
                direction.ToString()
            }) + ".anim";
            AnimationClip clip = (AnimationClip)AssetDatabase.LoadAssetAtPath(clipPath, typeof(AnimationClip));
            //blendTree.AddChild(clip, direction.ToVector2());
        }

    }


}
