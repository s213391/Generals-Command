using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    float secondsLeft;
    TextMeshProUGUI timerText;
    
    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponentInChildren<TextMeshProUGUI>();
        
        secondsLeft = 300f;
        timerText.text = "5:00";
    }

    // Update is called once per frame
    void Update()
    {

        if (secondsLeft > 0)
        {
            secondsLeft -= Time.deltaTime;
            if ((int)secondsLeft % 60 < 10)
                timerText.text = ((int)secondsLeft / 60).ToString() + ":0" + ((int)secondsLeft % 60).ToString();
            else
                timerText.text = ((int)secondsLeft / 60).ToString() + ":" + ((int)secondsLeft % 60).ToString();

            return;
        }

        int highestHealthPlayer = -1;
        int highestHealth = 0;
        for (int i = 0; i < GameData.instance.playerData.Count; i++)
        {
            if (GUIPlayerScore.instance.playerObjectHealths[i] > highestHealth)
            { 
                highestHealth = GUIPlayerScore.instance.playerObjectHealths[i];
                highestHealthPlayer = i;
            }
        }

        if (highestHealthPlayer == GameData.instance.localPlayerNumber)
            GameOver.instance.TriggerGameOver(true);
        else
            GameOver.instance.TriggerGameOver(false);
    }
}
