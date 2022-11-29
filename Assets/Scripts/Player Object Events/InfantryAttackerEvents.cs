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
        CombatManager.instance.CombatOccured();
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

            if (animator.GetBool("IsMoving"))
                animator.transform.localRotation = Quaternion.identity;
            else
            {
                //make unit face its target, avoiding NaN errors
                Vector3 angle = animator.transform.localEulerAngles;
                if (right == 0.0f)
                {
                    if (forward > 0.0f)
                        angle.y = 0.0f;
                    else
                        angle.y = 180.0f;
                }
                else
                    angle.y = Mathf.Atan2(forward, right);

                animator.transform.localEulerAngles = angle;
            }
        }
    }
}
