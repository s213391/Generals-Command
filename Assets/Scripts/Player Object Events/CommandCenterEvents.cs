using UnityEngine;
using RTSModularSystem;

public class CommandCenterEvents : AttackableEvents
{
    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
    }


    public override void OnDamage(int newHealth, int oldHealth)
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
        PlayOneShotAudio(_audioSource, deathSounds);
        StartParticleEffect(deathParticles);

        if (GameData.instance.isHost)
            SetAnimationTrigger(deathAnimators, "Death");

        if (GetComponent<PlayerObject>().owningPlayer == GameData.instance.localPlayerNumber + 1)
            GameOver.instance.TriggerGameOver(false);
        else
            GameOver.instance.TriggerGameOver(true);
    }
}
