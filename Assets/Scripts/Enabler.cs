using UnityEngine;
using Mirror;

public class Enabler : MonoBehaviour
{ 
    //spawn scene objects
    private void Start()
    {
        if (GameData.instance.isHost)
            NetworkServer.SpawnObjects();
    }
}
