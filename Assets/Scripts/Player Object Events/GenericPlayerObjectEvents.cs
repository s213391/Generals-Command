using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GenericPlayerObjectEvents : PlayerObjectEvents
{
    AudioSource _audioSource;

    public override void Init()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        base.Init();
    }


    [Server]
    public override void OnSpawn()
    {
        RpcOnSpawn();
    }


    [ClientRpc]
    public void RpcOnSpawn()
    {
        PlayOneShotAudio(_audioSource, spawnSounds);
        StartParticleEffect(spawnParticles);
    }
}
