using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using Mirror.Discovery;
using TMPro;

public class ServerLobby : NetworkBehaviour
{
    public static ServerLobby instance;

    [Header("Prefabs")]
    public GameObject playerSlotPrefab;

    [Header("Component References")]
    public NetworkDiscovery networkDiscovery;
    public GameObject connectingScreen;
    public GameObject gameStartingScreen;
    public Transform playerSlotParent;
    public TextMeshProUGUI playersCountText;
    public TextMeshProUGUI readyCountText;
    public Button readyButton;
    public Button unreadyButton;
    public GameObject blockoutPanel;
    public GameObject startGameButton;

    [Header("Internal Values")]
    [SerializeField]
    private int maxPlayerNumber = 4;
    [SerializeField]
    private int localPlayerNumber;
    [SerializeField]
    private bool clientsNeedUpdating = false;
    [SerializeField]
    private int clientsAcknowledgedStart;

    [SerializeField]
    private List<LobbyPlayer> lobbyPlayerData;
    [SerializeField]
    private List<NetworkConnection> playerConnections;
    [SerializeField]
    private List<LobbyPlayerUI> playerSlots;

    [SerializeField]
    private bool teamsAllowed = false;
    [SerializeField]
    private float productionMultiplier = 1.0f;
    [SerializeField]
    private float resourceMultiplier = 1.0f;
    [SerializeField]
    private int mapIndex = 0;


    //set up the singleton
    void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            Init();
    }


    //set up networking and visual elements
    void Init()
    {
        instance = this;
        
        networkDiscovery = FindObjectOfType<NetworkDiscovery>();
        lobbyPlayerData = new List<LobbyPlayer>();
        playerSlots = new List<LobbyPlayerUI>();

        clientsAcknowledgedStart = 0;
        gameStartingScreen.SetActive(false);
        connectingScreen.SetActive(true);

        if (GameData.instance.isHost)
        {
            playerConnections = new List<NetworkConnection>();
            connectingScreen.GetComponentInChildren<TextMeshProUGUI>().text = "Setting up Lobby";
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

    #region playerConnections

    [Server]
    //creates new data for the joining player
    public void PlayerConnect(NetworkConnection connection)
    { 
        LobbyPlayer player = new LobbyPlayer();
        player.playerNumber = playerConnections.Count;
        playerConnections.Add(connection);

        player.name = "Player " + playerConnections.Count;
        player.colourIndex = -1;
        player.teamIndex = -1;
        player.ready = false;
        lobbyPlayerData.Add(player);

        StartCoroutine(WaitToInitialiseClient(connection));
    }


    [Server]
    //waits a second before sending server settings to the client
    private IEnumerator WaitToInitialiseClient(NetworkConnection connection)
    {
        yield return new WaitForSeconds(1f);
        RpcSetLobbyValues(connection, connection.connectionId, maxPlayerNumber);
    }


    [TargetRpc]
    //tells the client basic server settings when they join
    private void RpcSetLobbyValues(NetworkConnection connection, int connID, int maxPlayers)
    {
        GameData.instance.SetLobbyValues(connID, maxPlayers);
        maxPlayerNumber = maxPlayers;

        for (int i = 0; i < maxPlayerNumber; i++)
        {
            playerSlots.Add(Instantiate(playerSlotPrefab, playerSlotParent).GetComponent<LobbyPlayerUI>());
            playerSlots[i].Init();
        }

        connectingScreen.SetActive(false);
        CmdClientHasInitialised();
    }


    [Command(requiresAuthority = false)]
    //sends a request to the server to update playerData on all clients when a new client finishes initialising the lobby
    private void CmdClientHasInitialised()
    {
        clientsNeedUpdating = true;
    }


    [Server]
    //removes the data for the disconnecting player from the host
    public void PlayerDisconnect(int connectionID)
    {
        for (int i = 0; i < playerConnections.Count; i++)
        {
            if (playerConnections[i].connectionId == connectionID)
            {
                lobbyPlayerData.RemoveAt(i);
                playerConnections.RemoveAt(i);
                clientsNeedUpdating = true;
                break;
            }
        }
    }


    [Server]
    //returns the player number for a given connection
    public int GetPlayerNumber(int connID)
    { 
        for (int i=0;i<playerConnections.Count;i++)
            if (playerConnections[i].connectionId == connID)
                return i;

        return -1;
    }


    //returns the data of the local player
    public LobbyPlayer GetLocalPlayerData()
    {
        return lobbyPlayerData[localPlayerNumber];
    }

    #endregion

    #region updatingVisuals


    //only runs on the server
    //if there have been any changes to the player data this frame, update all clients
    private void LateUpdate()
    {
        if (!isServer)
            return;

        if (clientsNeedUpdating)
        {
            if (!teamsAllowed)
                for (int i = 0; i < lobbyPlayerData.Count; i++)
                {
                    LobbyPlayer temp = lobbyPlayerData[i];
                    temp.teamIndex = i;
                    lobbyPlayerData[i] = temp;
                }
            
            for (int i = 1; i < playerConnections.Count; i++)
                RpcUpdatePlayerData(playerConnections[i], playerConnections[i].connectionId, i, lobbyPlayerData, teamsAllowed, productionMultiplier, resourceMultiplier);

            clientsNeedUpdating = false;
            UpdateMenu();
        }
    }


    [TargetRpc]
    //updates data on the client
    private void RpcUpdatePlayerData(NetworkConnection connection, int connID, int number, List<LobbyPlayer> lobbyPlayer, bool teams, float production, float resource)
    {
        GameData.instance.SetLobbyValues(connID, maxPlayerNumber);
        localPlayerNumber = number;
        lobbyPlayerData = lobbyPlayer;
        teamsAllowed = teams;
        productionMultiplier = production;
        resourceMultiplier = resource;
        UpdateMenu();
    }


    //updates all ui elements based on the new data on the server
    private void UpdateMenu()
    { 
        for (int i=0;i<playerSlots.Count;i++)
        {
            if (i < lobbyPlayerData.Count)
                playerSlots[i].UpdateData(true, lobbyPlayerData[i]);
            else
                playerSlots[i].UpdateData(false, new LobbyPlayer());
        }

        if (GetLocalPlayerData().ready)
        {
            readyButton.gameObject.SetActive(false);
            unreadyButton.gameObject.SetActive(true);
            blockoutPanel.gameObject.SetActive(true);
        }
        else
        {
            readyButton.gameObject.SetActive(true);
            unreadyButton.gameObject.SetActive(false);
            blockoutPanel.gameObject.SetActive(false);

            readyButton.interactable = CanReadyUp();
        }

        int readyCount = 0;
        foreach (LobbyPlayer player in lobbyPlayerData)
            if (player.ready)
                readyCount++;

        playersCountText.text = "Players (" + lobbyPlayerData.Count + "/"+ maxPlayerNumber + ")";
        readyCountText.text = "Players Ready (" + readyCount + "/" + lobbyPlayerData.Count + ")";

        if (GameData.instance.isHost && readyCount == lobbyPlayerData.Count)
            startGameButton.SetActive(true);
        else
            startGameButton.SetActive(false);
    }

    #endregion

    #region readyStatus

    //checks if player can ready up
    public bool CanReadyUp()
    { 
        LobbyPlayer player = GetLocalPlayerData();
        return (player.colourIndex != -1 && player.teamIndex != -1);
    }


    //updates player's ready status
    public void SetReadyStatus(bool ready)
    {
        CmdSetReadyStatus(GameData.instance.connectionID, ready);
    }


    [Command (requiresAuthority = false)]
    //tells the host whether this player is ready to start the game
    private void CmdSetReadyStatus(int connID, bool ready)
    {
        int playerNumber = GetPlayerNumber(connID);
        if (playerNumber != -1)
        {
            LobbyPlayer temp = lobbyPlayerData[playerNumber];
            temp.ready = ready;
            lobbyPlayerData[playerNumber] = temp;
        }
        clientsNeedUpdating = true;
    }

    #endregion

    #region playerNames

    //tells the server to update a player's name
    public void SetPlayerName(string name)
    {
        CmdSetPlayerName(GameData.instance.connectionID, name);
    }


    [Command(requiresAuthority = false)]
    //tells the host whether this player is ready to start the game
    private void CmdSetPlayerName(int connID, string name)
    {
        int playerNumber = GetPlayerNumber(connID);
        if (playerNumber != -1)
        {
            LobbyPlayer temp = lobbyPlayerData[playerNumber];
            temp.name = name;
            lobbyPlayerData[playerNumber] = temp;
        }
        clientsNeedUpdating = true;
    }

    #endregion

    #region dropdownLists

    //sets the chosen option on the server
    public void SetChosenOption(int listID, int optionIndex)
    {
        CmdSetChosenOption(GameData.instance.connectionID, listID, optionIndex);
    }


    //tells the server that this client has chosen an option
    [Command(requiresAuthority = false)]
    public void CmdSetChosenOption(int connID, int listID, int optionIndex)
    {
        int playerNumber = GetPlayerNumber(connID);

        if (listID == 0)
        {
            if (GetAvailableColours()[optionIndex])
                SetPlayerColour(connID, optionIndex);
            else
                return;
        }
        else
            SetTeamNumber(connID, optionIndex);
    }


    [Server]
    //set the player's new colour and update all clients
    public void SetPlayerColour(int connID, int colourIndex)
    {
        int playerNumber = GetPlayerNumber(connID);
        if (playerNumber != -1)
        {
            LobbyPlayer temp = lobbyPlayerData[playerNumber];
            temp.colourIndex = colourIndex;
            lobbyPlayerData[playerNumber] = temp;
        }
        clientsNeedUpdating = true;
    }


    [Server]
    //set the player's new team number and update all clients
    public void SetTeamNumber(int connID, int team)
    {
        int playerNumber = GetPlayerNumber(connID);
        if (playerNumber != -1)
        {
            LobbyPlayer temp = lobbyPlayerData[playerNumber];
            temp.teamIndex = team;
            lobbyPlayerData[playerNumber] = temp;
        }
        clientsNeedUpdating = true;
    }


    //returns the array of available options for colours
    public bool[] GetAvailableColours()
    {
        //set all options available
        bool[] available = new bool[playerSlots[0].GetDropdown(0).enabledOptions.Length];
        for (int i = 0; i < available.Length; i++)
            available[i] = true;

        //disable any options chosen by other players
        for (int i = 0; i < lobbyPlayerData.Count; i++)
            if (lobbyPlayerData[i].colourIndex != -1)
                available[lobbyPlayerData[i].colourIndex] = false;

        return available;
    }


    //get the colour value at a given index
    public Color IndexToColour(int index)
    {
        return playerSlots[0].GetDropdown(0).GetOptionButton(index).transform.GetChild(0).GetComponent<Image>().color;
    }


    //get the index value of a colour
    public int ColourToIndex(Color colour)
    {
        LockableDropdownList ndl = playerSlots[0].GetDropdown(0);
        for (int i = 0; i < ndl.enabledOptions.Length; i++)
        {
            if (ndl.GetOptionButton(i).transform.GetChild(0).GetComponent<Image>().color == colour)
                return i;
        }
        return -1;
    }


    //returns whether teams are enabled
    public bool TeamsEnabled()
    {
        return teamsAllowed;
    }

    #endregion

    #region startGame

    [Server]
    //move players to the map scene
    public void StartGame()
    {
        //prevent new clients joining after start
        networkDiscovery.StopDiscovery();
        NetworkManager.singleton.maxConnections = lobbyPlayerData.Count;

        //convert lobby data to player data
        List<PlayerData> playerDatas = new List<PlayerData>();
        for (int i = 0; i < lobbyPlayerData.Count; i++)
        {
            PlayerData playerData = new PlayerData();
            playerData.name = lobbyPlayerData[i].name;
            playerData.playerNumber = i;
            playerData.colour = IndexToColour(lobbyPlayerData[i].colourIndex);
            playerData.team = lobbyPlayerData[i].teamIndex + 1;

            playerDatas.Add(playerData);
        }

        for (int i = 0; i < lobbyPlayerData.Count; i++)
            RpcGameStarting(playerConnections[i], playerDatas);

        StartCoroutine(StartGameWhenAllClientsAcknowledge());
    }


    [Server]
    //start the game after all clients haved recieved and acknowledged settings updates and start
    private IEnumerator StartGameWhenAllClientsAcknowledge()
    {
        while (true)
        {
            if (clientsAcknowledgedStart == lobbyPlayerData.Count)
                break;
            else
                yield return null;
        }
        
        //NetworkManager.singleton.ServerChangeScene("GreyBox (Harry)");
        NetworkManager.singleton.ServerChangeScene("Nuclear wastefield map");
    }


    [TargetRpc]
    //set the game data for all clients and update the player prefab before scene change
    private void RpcGameStarting(NetworkConnection connection, List<PlayerData> playerDatas)
    {
        gameStartingScreen.SetActive(true);
        GameData.instance.SetGameValues(localPlayerNumber, playerDatas, productionMultiplier, resourceMultiplier);
        PlayerPrefabsManager.SetPlayerPrefab(0);
        NetworkManager.singleton.autoCreatePlayer = true;
        CmdAcknowledgeGameStart();
    }


    [Command(requiresAuthority = false)]
    //tell the server this client has acknowledged that the game is starting
    private void CmdAcknowledgeGameStart()
    {
        clientsAcknowledgedStart++;
    }

    #endregion
}
