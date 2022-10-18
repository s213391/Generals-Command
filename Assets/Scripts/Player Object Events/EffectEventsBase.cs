using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectEventsBase : MonoBehaviour
{
    #region sounds

    protected void PlayOneShotClip(GameObject go, AudioClip clip)
    {
        AudioSource audioSource = go.GetComponent<AudioSource>();
        if (audioSource == null)
            return;

        audioSource.volume = Settings.sfxVolume;
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }


    protected void StartSoundClip(GameObject go, AudioClip clip)
    {
        AudioSource audioSource = go.GetComponent<AudioSource>();
        if (audioSource == null)
            return;

        audioSource.volume = Settings.sfxVolume;
        if (clip != null && !audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }


    protected void StopSoundClip(GameObject go)
    {
        AudioSource audioSource = go.GetComponent<AudioSource>();
        if (audioSource == null)
            return;

        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    #endregion

    #region animations

    protected void SetAnimationBool(GameObject go, string boolName, bool value)
    {
        Animator[] animators = go.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
            animator.SetBool(boolName, value);
    }


    protected void SetAnimationTrigger(GameObject go, string triggerName)
    {
        Animator[] animators = go.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
            animator.SetTrigger(triggerName);
    }

    #endregion
}
