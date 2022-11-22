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


    public override void OnSpawn(bool localOwned)
    {
        if (localOwned)
        {
            PlayOneShotAudio(_audioSource, spawnSounds);
            StartParticleEffect(spawnParticles);
        }
    }
}
