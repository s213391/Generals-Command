using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Scriptable Objects/Events/AttackerEvents", order = 202)]
public class AttackerEvents : ScriptableObject
{
    public UnityEvent<GameObject> onAttack;
}
