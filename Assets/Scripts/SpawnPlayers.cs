using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnPlayers : MonoBehaviour
{
    public bool spawned = false;
    public GameObject objectDataManager;

    //spawn the host and scene objects
    private void Start()
    {
        if (NetworkClient.ready)
            spawned = NetworkClient.AddPlayer();
        if (GameData.instance.isHost)
            NetworkServer.SpawnObjects();
    }

    //spawn the player
    void Update()
    {
        if (NetworkClient.ready && !spawned)
            spawned = NetworkClient.AddPlayer();
    }
}
