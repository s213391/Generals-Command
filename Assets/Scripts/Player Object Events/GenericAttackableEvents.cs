using System.Collections;
using UnityEngine;
using RTSModularSystem;
using RTSModularSystem.BasicCombat;
using Mirror;

public class GenericAttackableEvents : AttackableEvents
{
    AudioSource _audioSource;
    float timeSinceAttacked = 20.0f;

    private void Update()
    {
        timeSinceAttacked += Time.deltaTime;
    }


    public override void Init()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        base.Init();
    }


    public override void OnDamage(int newHealth, int oldHealth)
    {
        RpcOnDamage(newHealth, oldHealth);
    }


    [ClientRpc]
    public void RpcOnDamage(int newHealth, int oldHealth)
    {
        PlayOneShotAudio(_audioSource, damageSounds);
        StartParticleEffect(damageParticles);

        if (timeSinceAttacked > 20.0f && RTSPlayer.Owns(GetComponent<PlayerObject>()))
            NotificationManager.instance.RequestNotification(3, GetComponent<PlayerObject>().data.name);

        timeSinceAttacked = 0.0f;
        CombatManager.instance.CombatOccured();
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
        StartCoroutine(DestroyAfterTime());
    }


    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }
}
