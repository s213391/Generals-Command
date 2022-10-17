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

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError($"No audio source detected on: {gameObject.name}");
            Destroy(this);
        }
        else
        {
            audioSource.loop = true;
            audioSource.clip = ongoingMoveClip;
        }
    }


    public void PlayDamageClip()
    {
        audioSource.volume = sfxVolume;
        if (damageClip != null)
            audioSource.PlayOneShot(damageClip);
    }


    public void PlayHealClip()
    {
        audioSource.volume = sfxVolume;
        if (healClip != null)
            audioSource.PlayOneShot(healClip);
    }


    public void PlayDeathClip()
    {
        audioSource.volume = sfxVolume;
        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);
    }


    public void PlayAttackClip()
    {
        audioSource.volume = sfxVolume;
        if (attackClip != null)
            audioSource.PlayOneShot(attackClip);
    }


    public void PlayBeginMovementClip()
    {
        audioSource.volume = sfxVolume;
        if (beginMoveClip != null)
            audioSource.PlayOneShot(beginMoveClip);
        if (ongoingMoveClip != null && !audioSource.isPlaying)
            audioSource.Play();
    }


    public void PlayEndMovementClip()
    {
        audioSource.volume = sfxVolume;
        if (endMoveClip != null)
            audioSource.PlayOneShot(endMoveClip);
        if (audioSource.isPlaying)
            audioSource.Stop();
    }


    public void PlaySelectedClip()
    {
        audioSource.volume = sfxVolume;
        if (selectedClip != null)
            audioSource.PlayOneShot(selectedClip);
    }
}
