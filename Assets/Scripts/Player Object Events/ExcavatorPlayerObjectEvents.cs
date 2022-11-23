using Mirror;
using UnityEngine;

public class ExcavatorPlayerObjectEvents : PlayerObjectEvents
{
    AudioSource _audioSource;

    public override void Init()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        base.Init();
    }


    public override void OnSpawn(bool localOwned)
    {
        if (localOwned)
        {
            PlayOneShotAudio(_audioSource, spawnSounds);
            StartParticleEffect(spawnParticles);
        }
        else
            NotificationManager.instance.RequestNotification(2);
    }
}
