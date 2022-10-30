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

    public abstract void OnDamage(int newHealth, int oldHealth);
    public abstract void OnHeal(int newHealth, int oldHealth);
    public abstract void OnDeath();
}
