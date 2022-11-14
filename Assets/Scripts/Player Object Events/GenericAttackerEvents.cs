using UnityEngine;
using Mirror;

public class GenericAttackerEvents : AttackerEvents
{
    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
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
