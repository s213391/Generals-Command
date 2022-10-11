using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public static GameOver instance;
    
    public GameObject winScreen;
    public GameObject loseScreen;

    private void Start()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }


    public void TriggerGameOver(bool win)
    {
        if (win)
            winScreen.SetActive(true);
        else
            loseScreen.SetActive(true);
    }
}
