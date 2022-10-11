using UnityEngine;
using Mirror;

public class PlayerPrefabsManager : MonoBehaviour
{
    public static PlayerPrefabsManager instance;
    
    public GameObject[] playerPrefabs = new GameObject[1];


    //set up the singletone
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }


    //replaces the current player prefab in the network manager
    public static void SetPlayerPrefab(int index)
    {
        if (instance.playerPrefabs.Length > index)
            NetworkManager.singleton.playerPrefab = instance.playerPrefabs[index];
    }
}
