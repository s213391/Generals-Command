using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct AttackableEvents
{
    public UnityEvent<GameObject, int, int> onDamage;
    public UnityEvent<GameObject, int, int> onHeal;
    public UnityEvent<GameObject> onDeath;
}
