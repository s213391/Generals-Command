using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;
using TMPro;


public class JoinGameMenu : MonoBehaviour
{
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    Vector2 scrollViewPos = Vector2.zero;

    public NetworkDiscovery networkDiscovery;

    public GameObject findLANGameButton;
    public GameObject serverList;
    public TextMeshProUGUI serverCountText;


#if UNITY_EDITOR
    void OnValidate()
    {
        if (networkDiscovery == null)
        {
            networkDiscovery = GetComponent<NetworkDiscovery>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
            UnityEditor.Undo.RecordObjects(new Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
        }
    }
#endif


    private GameObject currentButton;
    //update the list of local servers
    private void Update()
    {
        if (discoveredServers.Count < 1)
        {
            serverCountText.text = "No servers found";

            for (int i = 0; i < serverList.transform.childCount; i++)
                serverList.transform.GetChild(i).gameObject.SetActive(false);
        }
        else
        { 
            serverCountText.text = "Servers found: " + discoveredServers.Count.ToString();

            int i = 0;
            foreach (ServerResponse info in discoveredServers.Values)
            {
                currentButton = serverList.transform.GetChild(i).gameObject;
                currentButton.SetActive(true);
                currentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = info.EndPoint.Address.ToString();
                currentButton.GetComponent<Button>().onClick.RemoveAllListeners();
                currentButton.GetComponent<Button>().onClick.AddListener(delegate { Connect(info); });
                i++;
            }

            for (i = discoveredServers.Count; i < serverList.transform.childCount; i++)
                serverList.transform.GetChild(i).gameObject.SetActive(false);

        }
    }


    //starts checking the local network for open servers
    public void FindLANGame()
    {
        findLANGameButton.SetActive(false);
        serverList.SetActive(true);
        serverCountText.gameObject.SetActive(true);
        
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
    }


    //stops checking for servers
    public void Close()
    {
        findLANGameButton.SetActive(true);
        serverList.SetActive(false);
        serverCountText.gameObject.SetActive(false);

        discoveredServers.Clear();
        networkDiscovery.StopDiscovery();
    }


    //connect to the selected server
    void Connect(ServerResponse info)
    {
        networkDiscovery.StopDiscovery();
        NetworkManager.singleton.StartClient(info.uri);
    }


    //get advertised information from a discovered server
    public void OnDiscoveredServer(ServerResponse info)
    {
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        discoveredServers[info.serverId] = info;
    }
}
