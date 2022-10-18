using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct MovableEvents
{
    public UnityEvent<GameObject> onMoveBegin;
    public UnityEvent<GameObject> onMoveEnd;
}
