using UnityEngine;
using Mirror;

public abstract class EffectEventsBase : NetworkBehaviour 
{
    #region sounds


    protected void PlayOneShotAudio(AudioSource audioSource, AudioClip[] clips)
    {
        if (!audioSource || clips.Length == 0)
            return;
        else if (clips.Length == 1)
            audioSource.PlayOneShot(clips[0]);
        else
            audioSource.PlayOneShot(clips[(int)Random.Range(1, clips.Length)]);
    }


    protected void PlayOneShotAudio(AudioSource audioSource, AudioClip clip)
    {
        if (!audioSource)
            return;

        if (clip != null)
            audioSource.PlayOneShot(clip);
    }


    protected void PlayLoopingSound(AudioSource audioSource, AudioClip clip)
    {
        if (!audioSource)
            return;

        if (clip != null && !audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }


    protected void StopLoopingSound(AudioSource audioSource)
    {
        if (!audioSource)
            return;

        if (audioSource.clip != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    #endregion

    #region animations

    protected void SetAnimationBool(Animator[] animators, string boolName, bool value)
    {
        foreach (Animator animator in animators)
            animator.SetBool(boolName, value);
    }


    protected void SetAnimationTrigger(Animator[] animators, string triggerName)
    {
        foreach (Animator animator in animators)
            animator.SetTrigger(triggerName);
    }

    #endregion

    #region particles


    protected void StartParticleEffect(ParticleSystem[] emitters)
    {
        foreach (ParticleSystem emitter in emitters)
        {
            if (emitter && !emitter.isEmitting)
            {
                emitter.Play();
                for (int i = 0; i < emitter.transform.childCount; i++)
                    emitter.transform.GetChild(i).GetComponent<ParticleSystem>()?.Play();
            }
        }
        
    }


    protected void StopParticleEffect(ParticleSystem[] emitters)
    {
        foreach (ParticleSystem emitter in emitters)
        {
            if (emitter && emitter.isEmitting)
            {
                emitter.Stop();
                for (int i = 0; i < emitter.transform.childCount; i++)
                    emitter.transform.GetChild(i).GetComponent<ParticleSystem>()?.Stop();
            }
        }
    }

    #endregion
}
