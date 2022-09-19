using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Mirror.Discovery;
using TMPro;

public class ServerLobby : MonoBehaviour
{
    public NetworkDiscovery networkDiscovery;

    public GameObject startGameButton;
    public TextMeshProUGUI textMeshProUGUI;

    public int connections = 2;
    
    // Start is called before the first frame update
    void Start()
    {
        networkDiscovery = FindObjectOfType<NetworkDiscovery>();
        if (!GameData.instance.isHost)
            startGameButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameData.instance.isHost)
            connections = NetworkServer.connections.Count;
        textMeshProUGUI.text = "Players (" + NetworkServer.connections.Count + "/2";
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
}
