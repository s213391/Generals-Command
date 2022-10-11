using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public static GameData instance { get; private set; }

    public int connectionID;// { get; private set; }
    public bool isHost;// { get; private set; }
    public int localPlayerNumber;// { get; private set; }
    public int maxPlayerNumber;// { get; private set; }
    public List<PlayerData> playerData;// { get; private set; }
    public float productionMultiplier;// { get; private set; }
    public float resourceMultiplier;// { get; private set; }


    //set up singleton and set to persist across scenes
    void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    //sets host status in main menu
    public void SetHost(bool host)
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
            isHost = host;
    }


    //tells this client what its connectionID is
    public void SetLobbyValues(int connID, int maxPlayers)
    {
        connectionID = connID;
        maxPlayerNumber = maxPlayers;
    }  


    //sets game values in server lobby when game is ready to start
    public void SetGameValues(int playerNumber, List<PlayerData> info, float production, float resource)
    {
        if (SceneManager.GetActiveScene().name == "ServerLobby")
        {
            localPlayerNumber = playerNumber;
            playerData = info;
            productionMultiplier = production;
            resourceMultiplier = resource;
        }
    }


    //returns the data for this player
    public PlayerData GetLocalPlayerData()
    {
        return playerData[localPlayerNumber];
    }
}
