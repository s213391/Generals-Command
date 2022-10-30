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

    public abstract void OnAttack();
    public abstract void OnKill();
}
