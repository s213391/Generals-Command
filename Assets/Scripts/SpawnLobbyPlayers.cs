using UnityEngine;
using Mirror;

public class SpawnLobbyPlayers : MonoBehaviour
{
    public bool spawned;

    //spawn the host and scene objects
    private void Start()
    {
        spawned = false;
        if (NetworkClient.ready)
            spawned = NetworkClient.AddPlayer();
        if (GameData.instance.isHost)
            NetworkServer.SpawnObjects();
    }

    //spawn the player
    void Update()
    {
        if (!spawned && NetworkClient.ready)
            spawned = NetworkClient.AddPlayer();
    }
}
