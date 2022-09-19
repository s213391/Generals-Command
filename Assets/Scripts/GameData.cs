using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public static GameData instance { get; private set; }

    public bool isHost { get; private set; }
    public int playerNumber { get; private set; }
    public int teamNumber { get; private set; }
    public int colourIndex { get; private set; }
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
    public void SetGameValues(int team, int colour, float production, float resource)
    {
        if (SceneManager.GetActiveScene().name == "ServerLobby")
        {
            teamNumber = team;
            colourIndex = colour;
            productionMultiplier = production;
            resourceMultiplier = resource;
        }
    }
}
