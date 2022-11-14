using UnityEngine;
using Mirror;

public class GenericAttackerEvents : AttackerEvents
{
    AudioSource _audioSource;

    public override void Init()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        base.Init();
    }


    public override void OnAttack()
    {
        RpcOnAttack();
    }


    [ClientRpc]
    public void RpcOnAttack()
    {
        PlayOneShotAudio(_audioSource, attackSounds);
        StartParticleEffect(attackParticles);

        SetAnimationTrigger(animators, "Attacking");
    }


    public override void OnKill()
    {
        
    }
}
