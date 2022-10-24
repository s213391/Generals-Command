using UnityEngine;

public abstract class MovableEvents : EffectEventsBase
{
    public Animator[] animators;

    [Header("On Movement Begin")]
    public AudioClip[] movementBeginSounds;
    public ParticleSystem[] movementBeginParticles;

    [Header("On Movement End")]
    public AudioClip[] movementEndSounds;
    public ParticleSystem[] movementEndParticles;

    [Header("Movement Effects Played From Begin To End")]
    public AudioClip movementOngoingSounds;
    public ParticleSystem[] movementOngoingParticles;

    public abstract void OnMovementBegin();
    public abstract void OnMovementEnd();
}
