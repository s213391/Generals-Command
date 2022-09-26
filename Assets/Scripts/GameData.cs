using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public static GameData instance { get; private set; }


    public bool isHost { get; private set; }
    public int playerNumber { get; private set; }
    public List<LobbyPlayer> playerInfo { get; private set; }
    public float productionMultiplier { get; private set; }
    public float resourceMultiplier { get; private set; }


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


    //sets player number when joining server lobby
    public void SetPlayerNumber(int player)
    {
        if (SceneManager.GetActiveScene().name == "ServerLobby")
            playerNumber = player;
    }


    //sets game values in server lobby when game is ready to start
    public void SetGameValues(List<LobbyPlayer> info, float production, float resource)
    {
        if (SceneManager.GetActiveScene().name == "ServerLobby")
        {
            playerInfo = info;
            productionMultiplier = production;
            resourceMultiplier = resource;
        }
    }
}
