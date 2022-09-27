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

    [SerializeField]
    private List<LobbyPlayer> playerData = new List<LobbyPlayer>();
    [SerializeField]
    private List<NetworkConnection> playerIDs;
    private bool clientsNeedUpdating = false;
    private float productionMultiplier = 1.0f;
    private float resourceMultiplier = 1.0f;
    
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
        else
            playerIDs = new List<NetworkConnection>();
    }


    // Update is called once per frame
    void Update()
    {
        playersText.text = "Players (" + playerData.Count + "/2)";

        
    }


    //if there have been any changes to the player data this frame, update all clients
    private void LateUpdate()
    {
        if (!isServer)
            return;

        //if (clientsNeedUpdating)

    }


    [Server]
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
    public void PlayerConnect(NetworkConnection connection)
    { 
        LobbyPlayer player = new LobbyPlayer();
        playerIDs.Add(connection);

        player.team = NetworkServer.connections.Count;
        player.name = "Player " + player.team.ToString();
        player.colour = Color.black;
        player.ready = false;
        playerData.Add(player);
    }


    [Server]
    //removes the data for the disconnecting player from the host
    public void PlayerDisconnect(int connectionID)
    {
        for (int i = 0; i < playerIDs.Count; i++)
        {
            if (playerIDs[i].connectionId == connectionID)
            {
                playerData.RemoveAt(i);
                playerIDs.RemoveAt(i);
                clientsNeedUpdating = true;
                break;
            }
        }
    }


    [TargetRpc]
    //updates data on the client
    private void UpdatePlayerData(NetworkConnection connection, int number, List<LobbyPlayer> lobbyPlayer, float production, float resource)
    {
        GameData.instance.SetPlayerNumber(number);
        GameData.instance.SetGameValues(lobbyPlayer, production, resource);

    }


    public void Ready()
    {
        CmdReady(NetworkClient.connection.connectionId, GameData.instance.playerNumber);
    }


    [Command(requiresAuthority = false)]
    //tells the host that this player is ready to start the game
    private void CmdReady(int connectionID, int playerNumber)
    {
        if (connectionID == playerIDs[playerNumber].connectionId)
        {
            LobbyPlayer temp = playerData[playerNumber];
            temp.ready = true;
            playerData[playerNumber] = temp;
        }
    }
}
