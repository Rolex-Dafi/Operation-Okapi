using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScreenController : MonoBehaviour
{
    [SerializeField] private CanvasGroup toQuestionnaireGroup;
    [SerializeField] private CanvasGroup toSCIATGroup;

    private bool instructionsSwitched;
    
    private void Awake()
    {
        toQuestionnaireGroup.alpha = 1;
        toSCIATGroup.alpha = 0;
        toSCIATGroup.gameObject.SetActive(false);
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        // switch instructions when losing focus
        if (!hasFocus) SwitchInstructions();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        // switch instructions on pressing home button
        if (pauseStatus) SwitchInstructions();
    }
    
    public void GoToQst()
    {
        Application.OpenURL("https://diana.ms.mff.cuni.cz/formr/OcapiRun");
        
        SwitchInstructions();
    }
    
    public void GoTo2ndSciat()
    {
        // set variable 2nd sciat = true
        Utility.secondSciat = true;

        SceneManager.LoadScene("SC-IAT");
    }

    private void SwitchInstructions()
    {
        if (instructionsSwitched) return;
        
        // fade out upper
        toQuestionnaireGroup.DOFade(.3f, .3f);

        // show lower
        toSCIATGroup.gameObject.SetActive(true);
        toSCIATGroup.DOFade(1, .3f);

        instructionsSwitched = true;
    }
}
