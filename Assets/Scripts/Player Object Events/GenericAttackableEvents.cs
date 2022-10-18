using UnityEngine;
using RTSModularSystem;

public class GenericAttackableEvents : EffectEventsBase
{
    public void Death(GameObject gameobject)
    {
        gameobject.GetComponent<PlayerObject>().DestroyPlayerObject();

    }
}
