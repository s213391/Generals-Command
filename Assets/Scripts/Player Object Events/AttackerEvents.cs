using UnityEngine;

public abstract class AttackerEvents : EffectEventsBase
{
    public Animator[] animators;

    [Header("On Attack")]
    public AudioClip[] attackSounds;
    public ParticleSystem[] attackParticles;

    [Header("On Kill")]
    public AudioClip[] killSounds;
    public ParticleSystem[] killParticles;

    public virtual void Init()
    {
        foreach (ParticleSystem particle in attackParticles)
        {
            particle.gameObject.SetActive(true);
            for (int i = 0; i < particle.transform.childCount; i++)
                particle.transform.GetChild(i).gameObject.SetActive(true);
        }

        foreach (ParticleSystem particle in killParticles)
        {
            particle.gameObject.SetActive(true);
            for (int i = 0; i < particle.transform.childCount; i++)
                particle.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public abstract void OnAttack();
    public abstract void OnKill();
}
