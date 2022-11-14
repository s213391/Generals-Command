using UnityEngine;
using Mirror;

public class GenericMovableEvents : MovableEvents
{
    AudioSource _audioSource;

    public override void Init()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        base.Init();
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


    public override void OnUpdate(float forward, float right)
    {
        foreach (Animator animator in animators)
        {
            animator.SetFloat("forward", forward);
            animator.SetFloat("right", right);
        }
    }
}
