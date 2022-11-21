using UnityEngine;
using RTSModularSystem;
using RTSModularSystem.BasicCombat;
using Mirror;

public class GenericAttackableEvents : AttackableEvents
{
    AudioSource _audioSource;

    public override void Init()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        base.Init();
    }


    public override void OnDamage(int newHealth, int oldHealth)
    {
        CombatManager.instance.CombatOccured();
        NotificationManager.instance.RequestNotification(3, GetComponent<PlayerObject>().data.name);
        RpcOnDamage(newHealth, oldHealth);
    }


    [ClientRpc]
    public void RpcOnDamage(int newHealth, int oldHealth)
    {
        PlayOneShotAudio(_audioSource, damageSounds);
        StartParticleEffect(damageParticles);
    }


    public override void OnHeal(int newHealth, int oldHealth)
    {

    }


    public override void OnDeath()
    {
        RpcOnDeath();
    }


    [ClientRpc]
    public void RpcOnDeath()
    {
        PlayOneShotAudio(_audioSource, deathSounds);
        StartParticleEffect(deathParticles);

        SetAnimationTrigger(animators, "Death");

        GetComponent<PlayerObject>().DestroyPlayerObject();
    }
}
