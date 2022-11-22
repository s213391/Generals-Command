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


    [Server]
    public override void OnSpawn(bool localOwned)
    {
        if (localOwned)
            RpcOnSpawn();
        else
            NotificationManager.instance.RequestNotification(2);
    }


    [ClientRpc]
    public void RpcOnSpawn()
    {
        PlayOneShotAudio(_audioSource, spawnSounds);
        StartParticleEffect(spawnParticles);
    }
}
