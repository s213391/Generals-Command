using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Scriptable Objects/Events/MovableEvents", order = 203)]
public class MovableEvents : ScriptableObject
{
    public UnityEvent<GameObject> onMoveBegin;
    public UnityEvent<GameObject> onMoveEnd;
}
