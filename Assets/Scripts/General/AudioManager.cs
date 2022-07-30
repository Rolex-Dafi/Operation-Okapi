using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter[] ambientEmitters;
    [SerializeField] private EventReference buttonClick;

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
