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
    private List<NetworkConnection> playerConnections;
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
        {
            playerConnections = new List<NetworkConnection>();
            playerConnections.Add(NetworkClient.connection);
        }
    }


    //only runs on the server
    //if there have been any changes to the player data this frame, update all clients
    private void LateUpdate()
    {
        if (!isServer)
            return;

        if (clientsNeedUpdating)
        { 
            for (int i = 1; i < playerConnections.Count; i++)
            {
                RpcUpdatePlayerData(playerConnections[i], i, playerData, productionMultiplier, resourceMultiplier);
            }

            clientsNeedUpdating = false;
            UpdateMenu();
        }
    }


    //returns to menu
    public void BackButton()
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

    #region PlayerConnection

    [Server]
    //creates new data for the joining player
    public void PlayerConnect(NetworkConnection connection)
    { 
        LobbyPlayer player = new LobbyPlayer();
        playerConnections.Add(connection);

        player.team = playerConnections.Count;
        player.name = "Player joining";
        player.colour = Color.black;
        player.ready = false;
        playerData.Add(player);
    }


    [Server]
    //removes the data for the disconnecting player from the host
    public void PlayerDisconnect(int connectionID)
    {
        for (int i = 0; i < playerConnections.Count; i++)
        {
            if (playerConnections[i].connectionId == connectionID)
            {
                playerData.RemoveAt(i);
                playerConnections.RemoveAt(i);
                clientsNeedUpdating = true;
                break;
            }
        }
    }

    #endregion

    #region UpdatingVisuals

    [TargetRpc]
    //updates data on the client
    private void RpcUpdatePlayerData(NetworkConnection connection, int number, List<LobbyPlayer> lobbyPlayer, float production, float resource)
    {
        GameData.instance.SetPlayerNumber(number);
        GameData.instance.SetGameValues(lobbyPlayer, production, resource);
        UpdateMenu();
    }


    //updates all ui elements based on the new data on the server
    private void UpdateMenu()
    { 
        
    }

    #endregion

    #region PlayerColour

    //tell the server this player has selected a new colour
    public void SetPlayerColour(Color colour)
    {
        CmdSetPlayerColour(NetworkClient.connection.connectionId, GameData.instance.playerNumber, colour);
    }


    [Command(requiresAuthority = false)]
    //set the player's new colour and update all clients
    private void CmdSetPlayerColour(int connectionID, int playerNumber, Color colour)
    {
        if (connectionID == playerConnections[playerNumber].connectionId)
        {
            LobbyPlayer temp = playerData[playerNumber];
            temp.colour = colour;
            playerData[playerNumber] = temp;
        }
        clientsNeedUpdating = true;
    }

    #endregion

    #region TeamNumber

    //tell the server this player has selected a new team
    public void SetTeamNumber(int team)
    {
        CmdSetTeamNumber(NetworkClient.connection.connectionId, GameData.instance.playerNumber, team);
    }


    [Command(requiresAuthority = false)]
    //set the player's new team number and update all clients
    private void CmdSetTeamNumber(int connectionID, int playerNumber, int team)
    {
        if (connectionID == playerConnections[playerNumber].connectionId)
        {
            LobbyPlayer temp = playerData[playerNumber];
            temp.team = team;
            playerData[playerNumber] = temp;
        }
        clientsNeedUpdating = true;
    }

    #endregion

    #region ReadyStatus

    public void ReadyStatus(bool ready)
    {
        CmdReadyStatus(NetworkClient.connection.connectionId, GameData.instance.playerNumber, ready);
    }


    [Command(requiresAuthority = false)]
    //tells the host that this player is ready to start the game
    private void CmdReadyStatus(int connectionID, int playerNumber, bool ready)
    {
        if (connectionID == playerConnections[playerNumber].connectionId)
        {
            LobbyPlayer temp = playerData[playerNumber];
            temp.ready = ready;
            playerData[playerNumber] = temp;
        }
        clientsNeedUpdating = true;
    }

    #endregion


    [Server]
    //move players to the map scene
    public void StartGame()
    {
        NetworkManager.singleton.ServerChangeScene("GreyBox (Harry)");
    }
}
