using UnityEngine;

public abstract class AttackableEvents : EffectEventsBase
{
    public Animator[] animators;
    
    [Header("On Damage")]
    public AudioClip[] damageSounds;
    public ParticleSystem[] damageParticles;

    [Header("On Heal")]
    public AudioClip[] healSounds;
    public ParticleSystem[] healParticles;

    [Header("On Death")]
    public AudioClip[] deathSounds;
    public ParticleSystem[] deathParticles;

    public virtual void Init()
    {
        foreach (ParticleSystem particle in damageParticles)
        {
            particle.gameObject.SetActive(true);
            for (int i = 0; i < particle.transform.childCount; i++)
                particle.transform.GetChild(i).gameObject.SetActive(true);
        }

        foreach (ParticleSystem particle in healParticles)
        {
            particle.gameObject.SetActive(true);
            for (int i = 0; i < particle.transform.childCount; i++)
                particle.transform.GetChild(i).gameObject.SetActive(true);
        }

        foreach (ParticleSystem particle in deathParticles)
        {
            particle.gameObject.SetActive(true);
            for (int i = 0; i < particle.transform.childCount; i++)
                particle.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public abstract void OnDamage(int newHealth, int oldHealth);
    public abstract void OnHeal(int newHealth, int oldHealth);
    public abstract void OnDeath();
}
