using UnityEngine;

public abstract class AttackerEvents : EffectEventsBase
{
    [Header("On Attack")]
    public AudioClip[] attackSounds;
    public Animator[] attackAnimators;
    public ParticleSystem[] attackParticles;

    [Header("On Kill")]
    public AudioClip[] killSounds;
    public Animator[] killAnimators;
    public ParticleSystem[] killParticles;

    public abstract void OnAttack();
    public abstract void OnKill();
}
