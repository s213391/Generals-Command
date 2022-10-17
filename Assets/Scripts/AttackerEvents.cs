using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct AttackerEvents
{
    public UnityEvent<GameObject> onAttack;
    public UnityEvent<GameObject> onKill;
}
