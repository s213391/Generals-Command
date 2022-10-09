using System.Collections;
using UnityEngine;
using Mirror;
using RTSModularSystem;

public class HideLoadingScreen : MonoBehaviour
{
    public GameObject loadingScreen;

    private void Start()
    {
        loadingScreen.SetActive(true);
    }


    private void Update()
    {
        if (!loadingScreen.activeInHierarchy)
            return;

        //once this client has initialised its player disables loading screen
        if (RTSPlayer.localPlayer != null)
        {
            loadingScreen.SetActive(false);
            enabled = false;
        }
    }
}
