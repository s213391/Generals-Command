using UnityEngine;

public abstract class PlayerObjectEvents : EffectEventsBase
{
    public Animator[] animators;

    [Header("On Spawn")]
    public AudioClip[] spawnSounds;
    public ParticleSystem[] spawnParticles;

    public virtual void Init()
    {
        foreach (ParticleSystem particle in spawnParticles)
        {
            particle.gameObject.SetActive(true);
            for (int i = 0; i < particle.transform.childCount; i++)
                particle.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public abstract void OnSpawn(bool localOwned);
}
