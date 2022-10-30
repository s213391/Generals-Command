using UnityEngine;

public class GenericMovableEvents : MovableEvents
{
    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
    }


    public override void OnMovementBegin()
    {
        PlayOneShotAudio(_audioSource, movementBeginSounds);
        PlayLoopingSound(_audioSource, movementOngoingSounds);
        StartParticleEffect(movementBeginParticles);
        StartParticleEffect(movementOngoingParticles);

        if (GameData.instance.isHost)
            SetAnimationBool(animators, "IsMoving", true);
    }


    public override void OnMovementEnd()
    {
        StopLoopingSound(_audioSource);
        SetAnimationBool(animators, "IsMoving", false);
        StopParticleEffect(movementOngoingParticles);

        if (GameData.instance.isHost)
            SetAnimationBool(animators, "IsMoving", false);
    }
}
