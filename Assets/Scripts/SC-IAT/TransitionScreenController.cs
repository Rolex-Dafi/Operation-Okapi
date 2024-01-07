using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A controller for the transition screen which redirects the player to the questionnaires and the SC-IAT.
/// </summary>
public class TransitionScreenController : MonoBehaviour
{
    [SerializeField] private CanvasGroup toQuestionnaireGroup;
    [SerializeField] private CanvasGroup toSCIATGroup;

    [SerializeField] private DataSaver dataSaver;
    
    private bool instructionsSwitched;
    
    private void Awake()
    {
        dataSaver.SaveAnalytics();
        
        toQuestionnaireGroup.alpha = 1;
        toSCIATGroup.alpha = 0;
        toSCIATGroup.gameObject.SetActive(false);

        StartCoroutine(SwitchAfterTime());
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // switch instructions when losing focus
        if (!hasFocus) SwitchInstructions();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // switch instructions on pressing home button
        if (pauseStatus) SwitchInstructions();
    }
    
    /// <summary>
    /// Opens the questionnaire for the study.
    /// </summary>
    public void GoToQst()
    {
        Application.OpenURL("https://diana.ms.mff.cuni.cz/formr/OcapiRun");
        
        SwitchInstructions();
    }
    
    /// <summary>
    /// Loads the second SC-IAT scene.
    /// </summary>
    public void GoTo2ndSciat()
    {
        // set variable 2nd sciat = true
        Utility.secondSciat = true;

        SceneManager.LoadScene("SC-IAT");
    }

    private IEnumerator SwitchAfterTime()
    {
        // switch instructions after one minute - to be safe
        yield return new WaitForSeconds(60);

        SwitchInstructions();
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
