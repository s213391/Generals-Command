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

    protected void OnStart()
    {
        foreach (ParticleSystem particle in movementBeginParticles)
            particle.gameObject.SetActive(true);

        foreach (ParticleSystem particle in movementEndParticles)
            particle.gameObject.SetActive(true);

        foreach (ParticleSystem particle in movementOngoingParticles)
            particle.gameObject.SetActive(true);
    }

    public abstract void OnMovementBegin();
    public abstract void OnMovementEnd();
}
