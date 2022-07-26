using UnityEngine;
using RTSModularSystem;
using RTSModularSystem.BasicCombat;
using Mirror;

public class CommandCenterEvents : AttackableEvents
{
    AudioSource _audioSource;

    public override void Init()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        base.Init();
    }


    public override void OnDamage(int newHealth, int oldHealth)
    {
        NotificationManager.instance.RequestNotification(0);
        CombatManager.instance.CombatOccured();
        RpcOnDamage(newHealth, oldHealth);
    }


    [ClientRpc]
    public void RpcOnDamage(int newHealth, int oldHealth)
    {
        PlayOneShotAudio(_audioSource, damageSounds);
        StartParticleEffect(damageParticles);

        GUIPlayerScore.instance.UpdateHealth((int)(GetComponent<PlayerObject>().owningPlayer - 1), newHealth);
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

        if (GetComponent<PlayerObject>().owningPlayer == GameData.instance.localPlayerNumber + 1)
            GameOver.instance.TriggerGameOver(false);
        else
            GameOver.instance.TriggerGameOver(true);
    }
}
