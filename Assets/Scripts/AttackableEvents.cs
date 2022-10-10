using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Scriptable Objects/Events/AttackableEvents", order = 201)]
public class AttackableEvents : ScriptableObject
{
    public UnityEvent<GameObject, int, int> onDamage;
    public UnityEvent<GameObject, int, int> onHeal;
    public UnityEvent<GameObject> onDeath;
}
