using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.UI;

/// <summary>
/// Manages the ambience and the UI sounds in the game.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter[] ambientEmitters;
    [SerializeField] private EventReference buttonClick;

    public void Refresh()
    {
        FindButtons();
    }

    private void FindButtons()
    {
        var uiButtons = FindObjectsOfType<Button>(true);
        foreach (var button in uiButtons)
        {
            button.onClick.AddListener(PlayButtonClick);
        }
    }

    public void StartAmbience(int sceneIndex)
    {
        // offset by 1 bc the first scene (=0) is the base
        // stop the previous ambience first
        if (sceneIndex > 1) ambientEmitters[sceneIndex - 2].Stop();

        ambientEmitters[sceneIndex - 1].Play();
    }

    public void PlayButtonClick()
    {
        PlaySound(buttonClick.Guid);
    }

    public void PlaySound(FMOD.GUID id)
    {
        RuntimeManager.PlayOneShot(id);
    }
}
