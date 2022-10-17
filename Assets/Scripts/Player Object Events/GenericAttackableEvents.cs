using UnityEngine;
using RTSModularSystem;

public class GenericAttackableEvents : MonoBehaviour
{
    public void Death(GameObject gameobject)
    {
        gameobject.GetComponent<PlayerObject>().DestroyPlayerObject();

    }
}
