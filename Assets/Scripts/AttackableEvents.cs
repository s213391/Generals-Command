using UnityEngine;

public abstract class AttackableEvents : EffectEventsBase
{
    [Header("On Damage")]
    public AudioClip[] damageSounds;
    public Animator[] damageAnimators;
    public ParticleSystem[] damageParticles;

    [Header("On Heal")]
    public AudioClip[] healSounds;
    public Animator[] healAnimators;
    public ParticleSystem[] healParticles;

    [Header("On Death")]
    public AudioClip[] deathSounds;
    public Animator[] deathAnimators;
    public ParticleSystem[] deathParticles;

    public abstract void OnDamage(int newHealth, int oldHealth);
    public abstract void OnHeal(int newHealth, int oldHealth);
    public abstract void OnDeath();
}
