using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Mirror.Discovery;

public class HostGameMenu : MonoBehaviour
{
    public NetworkDiscovery networkDiscovery;
    
    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            NetworkManager.singleton.StartHost();
            networkDiscovery.AdvertiseServer();
            SceneManager.LoadScene("ServerLobby");
        }
    }
}
