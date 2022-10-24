using UnityEngine;

public abstract class MovableEvents : EffectEventsBase
{
    [Header("On Movement Begin")]
    public AudioClip[] movementBeginSounds;
    public Animator[] movementBeginAnimators;
    public ParticleSystem[] movementBeginParticles;

    [Header("On Movement End")]
    public AudioClip[] movementEndSounds;
    public Animator[] movementEndAnimators;
    public ParticleSystem[] movementEndParticles;

    [Header("Movement Effects Played From Begin To End")]
    public AudioClip movementOngoingSounds;
    public Animator[] movementOngoingAnimators;
    public ParticleSystem[] movementOngoingParticles;

    public abstract void OnMovementBegin();
    public abstract void OnMovementEnd();
}
