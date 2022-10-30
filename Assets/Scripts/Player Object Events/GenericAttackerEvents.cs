using UnityEngine;

public class GenericAttackerEvents : AttackerEvents
{
    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
    }


    public override void OnAttack()
    {
        PlayOneShotAudio(_audioSource, attackSounds);
        StartParticleEffect(attackParticles);

        if (GameData.instance.isHost)
            SetAnimationTrigger(animators, "Attack");
    }


    public override void OnKill()
    {
        
    }
}
