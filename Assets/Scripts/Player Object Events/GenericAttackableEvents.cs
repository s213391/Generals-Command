using UnityEngine;
using RTSModularSystem;

public class GenericAttackableEvents : AttackableEvents
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

        GetComponent<PlayerObject>().DestroyPlayerObject();
    }
}
