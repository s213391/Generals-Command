using UnityEngine;
using RTSModularSystem;
using Mirror;

public class CommandCenterEvents : AttackableEvents
{
    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
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

        GUIPlayerScore.instance.UpdateHealth((int)(GetComponent<PlayerObject>().owningPlayer - 1), (newHealth / 10));
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
