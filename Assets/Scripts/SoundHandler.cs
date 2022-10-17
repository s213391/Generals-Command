using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    [Header("Combat")]
    public AudioClip damageClip;
    public AudioClip healClip;
    public AudioClip deathClip;
    public AudioClip attackClip;

    [Header("Movement")]
    public AudioClip beginMoveClip;
    public AudioClip ongoingMoveClip;
    public AudioClip endMoveClip;

    [Header("Selection")]
    public AudioClip selectedClip;

    public static float sfxVolume;


    public void PlayDamageClip(GameObject go)
    {
        AudioSource audioSource = go.GetComponent<AudioSource>();
        if (audioSource == null)
            return;
        
        audioSource.volume = sfxVolume;
        if (damageClip != null)
            audioSource.PlayOneShot(damageClip);
    }


    public void PlayHealClip(GameObject go)
    {
        AudioSource audioSource = go.GetComponent<AudioSource>();
        if (audioSource == null)
            return;

        audioSource.volume = sfxVolume;
        if (healClip != null)
            audioSource.PlayOneShot(healClip);
    }


    public void PlayDeathClip(GameObject go)
    {
        AudioSource audioSource = go.GetComponent<AudioSource>();
        if (audioSource == null)
            return;

        audioSource.volume = sfxVolume;
        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);
    }


    public void PlayAttackClip(GameObject go)
    {
        AudioSource audioSource = go.GetComponent<AudioSource>();
        if (audioSource == null)
            return;

        audioSource.volume = sfxVolume;
        if (attackClip != null)
            audioSource.PlayOneShot(attackClip);
    }


    public void PlayBeginMovementClip(GameObject go)
    {
        AudioSource audioSource = go.GetComponent<AudioSource>();
        if (audioSource == null)
            return;

        audioSource.clip = ongoingMoveClip;
        audioSource.loop = true;

        audioSource.volume = sfxVolume;
        if (beginMoveClip != null)
            audioSource.PlayOneShot(beginMoveClip);
        if (ongoingMoveClip != null && !audioSource.isPlaying)
            audioSource.Play();
    }


    public void PlayEndMovementClip(GameObject go)
    {
        AudioSource audioSource = go.GetComponent<AudioSource>();
        if (audioSource == null)
            return;

        audioSource.volume = sfxVolume;
        if (endMoveClip != null)
            audioSource.PlayOneShot(endMoveClip);
        if (audioSource.isPlaying)
            audioSource.Stop();
    }


    public void PlaySelectedClip(GameObject go)
    {
        AudioSource audioSource = go.GetComponent<AudioSource>();
        if (audioSource == null)
            return;

        audioSource.volume = sfxVolume;
        if (selectedClip != null)
            audioSource.PlayOneShot(selectedClip);
    }
}
