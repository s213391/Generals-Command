using UnityEngine;
using Mirror;

public class GenericMovableEvents : MovableEvents
{
    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        OnStart();
    }


    public override void OnMovementBegin()
    {
        RpcOnMovementBegin();
    }


    [ClientRpc]
    public void RpcOnMovementBegin()
    {
        PlayOneShotAudio(_audioSource, movementBeginSounds);
        PlayLoopingSound(_audioSource, movementOngoingSounds);
        StartParticleEffect(movementBeginParticles);
        StartParticleEffect(movementOngoingParticles);

        SetAnimationBool(animators, "IsMoving", true);
    }


    public override void OnMovementEnd()
    {
        if (GameData.instance.isHost)
            RpcOnMovementEnd();
    }


    [ClientRpc]
    public void RpcOnMovementEnd()
    {
        StopLoopingSound(_audioSource);
        SetAnimationBool(animators, "IsMoving", false);
        StopParticleEffect(movementOngoingParticles);

        SetAnimationBool(animators, "IsMoving", false);
    }
}
