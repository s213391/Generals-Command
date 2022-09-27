using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Mirror.Discovery;
using TMPro;

public class ServerLobby : NetworkBehaviour
{
    public static ServerLobby instance;
    
    public NetworkDiscovery networkDiscovery;

    public GameObject startGameButton;
    public TextMeshProUGUI playersText;

    public readonly SyncList<LobbyPlayer> playerData = new SyncList<LobbyPlayer>();
    
    //set up the singleton
    void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            Init();
    }


    //set up visual elements
    void Init()
    {
        instance = this;
        networkDiscovery = FindObjectOfType<NetworkDiscovery>();


        if (!GameData.instance.isHost)
            startGameButton.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        playersText.text = "Players (" + playerData.Count + "/2)";

        //the host does not change player number, they will always be player 0
        if (!GameData.instance.isHost)
        {
            for (int i = 1; i < playerData.Count; i++)
            {
                if (playerData[i].connectionID == NetworkClient.connection.connectionId)
                {
                    if (GameData.instance.playerNumber != i)
                    {
                        GameData.instance.SetPlayerNumber(i);
                    }
                }
            }
        }
    }


    //move players to greybox map
    public void ChangeScene()
    {
        NetworkManager.singleton.ServerChangeScene("GreyBox (Harry)");
    }


    //returns to menu
    public void Back()
    {
        if (GameData.instance)
        {
            if (GameData.instance.isHost)
            {
                NetworkManager.singleton.StopHost();
                networkDiscovery.StopDiscovery();
            }
            else
            {
                NetworkManager.singleton.StopClient();
                networkDiscovery.StopDiscovery();
            }
        }

        SceneManager.LoadScene("MainMenu");
    }


    [Server]
    //creates new data for the joining player
    public void PlayerConnect(int connectionID)
    { 
        LobbyPlayer player = new LobbyPlayer();
        playerData.Add(player);

        player.connectionID = connectionID;
        player.team = NetworkServer.connections.Count;
        player.name = "Player " + player.team.ToString();
        player.colour = Color.black;
        player.ready = false;
    }


    [Server]
    //removes the data for the disconnecting player
    public void PlayerDisconnect(int connectionID)
    {
        foreach (LobbyPlayer player in playerData)
            if (player.connectionID == connectionID)
                playerData.Remove(player);
    }


    public void Ready()
    {
        CmdReady(NetworkClient.connection.connectionId);
    }


    [Command(requiresAuthority = false)]
    //tells the host that this player is ready to start the game
    private void CmdReady(int connectionID)
    {
        for (int i = 0; i < playerData.Count; i++)
        {
            if (playerData[i].connectionID == connectionID)
            {
                LobbyPlayer temp = playerData[i];
                temp.ready = true;
                playerData[i] = temp;
            }
        }
    }
}
