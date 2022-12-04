using RTSModularSystem.BasicCombat;
using UnityEngine;
using Mirror;

public class InfantryAttackerEvents : AttackerEvents
{
    AudioSource _audioSource;

    public override void Init()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        base.Init();
    }


    public override void OnTarget()
    {
        RpcOnTarget();
    }


    [ClientRpc]
    public void RpcOnTarget()
    {
        PlayOneShotAudio(_audioSource, targetSounds);
        StartParticleEffect(targetParticles);
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
        CombatManager.instance.CombatOccured();

        SetAnimationTrigger(animators, "Attacking");
    }


    public override void OnKill()
    {

    }


    public override void OnUpdate(float forward, float right)
    {
        RpcOnUpdate(forward, right);
    }


    [ClientRpc]
    public void RpcOnUpdate(float forward, float right)
    {
        foreach (Animator animator in animators)
        {
            animator.SetFloat("ForwardAiming", forward);
            animator.SetFloat("RightAiming", right);

            //make unit face its target
            Vector3 angle = animator.transform.localEulerAngles;
            angle.y = (Mathf.Atan2(right, forward) * Mathf.Rad2Deg);
            animator.transform.localEulerAngles = angle;
        }
    }
}
